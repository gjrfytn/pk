﻿using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace SharedClasses
{
    public static class Utility
    {
        public static readonly Dictionary<string, uint> DirCodesEduLevels = new Dictionary<string, uint>
        {
            {"03", 2}, //Бакалавриат
            {"05", 5}, //Специалитет
            {"04", 4}  //TODO Магистратура?
        };

        /// <summary>
        /// Отображает диалоговое окно с кнопками "Да" и "Нет".
        /// </summary>
        /// <param name="description">Текст сообщения.</param>
        /// <param name="caption">Текст заголовка.</param>
        /// <returns><c>true</c>, если нажата кнопка "Да".</returns>
        public static bool ShowChoiceMessageBox(string description, string caption)
        {
            return MessageBox.Show(
                description,
                caption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
                ) == DialogResult.Yes;
        }

        /// <summary>
        /// Отображает диалоговое окно, предупреждающее о невозможности отмены дейтсвия, с кнопками "Да" и "Нет".
        /// </summary>
        /// <returns><c>true</c>, если нажата кнопка "Да".</returns>
        public static bool ShowUnrevertableActionMessageBox()
        {
            return MessageBox.Show(
                  "Действие невозможно будет отменить. Вы уверены?",
                  "Внимание",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning,
                  MessageBoxDefaultButton.Button2
                  ) == DialogResult.Yes;
        }

        /// <summary>
        /// Отображает диалоговое окно, запрашивающее подтверждение закрытия формы, с кнопками "Да" и "Нет".
        /// </summary>
        /// <returns><c>true</c>, если нажата кнопка "Да".</returns>
        public static bool ShowFormCloseMessageBox()
        {
            return MessageBox.Show(
                  "Вы уверены, что хотите закрыть форму?",
                  "Внимание",
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Warning,
                  MessageBoxDefaultButton.Button2
                  ) == DialogResult.Yes;
        }

        /// <summary>
        /// Вызывает <see cref="ShowChoiceMessageBox(string, string)"/>. Далее, если нажата кнопка "Да", - <see cref="ShowUnrevertableActionMessageBox"/>.
        /// Если во втором окне нажата конпка "Нет", то снова вызывается первое.
        /// </summary>
        /// <param name="description">Текст сообщения для первого окна.</param>
        /// <param name="caption">Текст заголовка для первого окна.</param>
        /// <returns><c>true</c>, если в обоих окнах нажата кнопка "Да".</returns>
        public static bool ShowChoiceMessageWithConfirmation(string description, string caption)
        {
            while (true)
                if (ShowChoiceMessageBox(description, caption))
                {
                    if (ShowUnrevertableActionMessageBox())
                        return true;
                }
                else
                    return false;
        }

        /// <summary>
        /// Отображает окно, сообщающее об успешном сохранении данных.
        /// </summary>
        public static void ShowChangesSavedMessage()
        {
            MessageBox.Show("Изменения успешно сохранены.", "Сохранено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Распределяет абитуриентов по экзаменационным аудиториям.
        /// </summary>
        /// <param name="rooms">Аудитории (номер, кол-во мест).</param>
        /// <param name="letters">Абитуриенты (первая буква фамилии, количество).</param>
        /// <returns>Ассоциации (первая буква фамилии, номер аудитории). Если буквы повторяются то значит, что одна аудитория не вмещает всех с этой буквой.</returns>
        public static List<System.Tuple<char, string>> DistributeAbiturients(Dictionary<string, ushort> rooms, Dictionary<char, ushort> letters)
        {
            #region Contracts
            if (rooms == null)
                throw new System.ArgumentNullException(nameof(rooms));
            if (letters == null)
                throw new System.ArgumentNullException(nameof(letters));
            if (rooms.Count == 0)
                throw new System.ArgumentException("Словарь с аудиториями должен содержать хотя бы один элемент.", nameof(rooms));
            if (letters.Count == 0)
                throw new System.ArgumentException("Словарь с группами должен содержать хотя бы один элемент.", nameof(letters));
            if (rooms.Sum(r => r.Value) < letters.Sum(l => l.Value))
                throw new System.ArgumentException("Общее количество мест в аудиториях меньше общего количества абитуриентов.");
            #endregion

            Dictionary<string, ushort> roomsCopy = new Dictionary<string, ushort>(rooms);
            Dictionary<char, ushort> lettersCopy = letters.OrderByDescending(l => l.Value).ToDictionary(k => k.Key, v => v.Value);
            List<System.Tuple<char, string>> distributions = new List<System.Tuple<char, string>>();
            while (lettersCopy.Count != 0)
            {
                Dictionary<char, ushort> excludedLetters = new Dictionary<char, ushort>();
                string roomBuf = null;
                var room = roomsCopy.First();
                while (roomBuf == null)
                {
                    if (lettersCopy.Sum(l => l.Value) > room.Value)
                    {
                        char letter = lettersCopy.Last().Key;
                        if (lettersCopy.Count == 1)
                        {
                            var sufficientRoom = roomsCopy.FirstOrDefault(r => r.Value >= lettersCopy[letter]);
                            if (sufficientRoom.Value == 0)
                            {
                                excludedLetters.Add(letter, (ushort)(lettersCopy[letter] - room.Value));
                                lettersCopy[letter] = room.Value;
                            }
                            else
                                roomBuf = sufficientRoom.Key;
                        }
                        else
                        {
                            excludedLetters.Add(letter, lettersCopy[letter]);
                            lettersCopy.Remove(letter);
                        }
                    }
                    else
                        roomBuf = room.Key;
                }

                roomsCopy[roomBuf] -= (ushort)lettersCopy.Sum(l => l.Value);
                if (roomsCopy[roomBuf] == 0)
                    roomsCopy.Remove(roomBuf);

                foreach (var letter in lettersCopy)
                    distributions.Add(new System.Tuple<char, string>(letter.Key, roomBuf));

                lettersCopy.Clear();

                foreach (var el in excludedLetters)
                    lettersCopy.Add(el.Key, el.Value);
                lettersCopy = lettersCopy.OrderByDescending(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
            }

            return distributions;
        }

        public static void Print(string file)
        {
            #region Contracts
            if (string.IsNullOrWhiteSpace(file))
                throw new System.ArgumentException("Некорректное имя файла.", nameof(file));
            #endregion

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(file);
            info.Verb = "Print";
            info.CreateNoWindow = true;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process.Start(info);
            //System.Diagnostics.Process.Start(file);

            //p.WaitForExit();надо?
            //p.Close();?
            //p.Dispose();?
        }

        public static bool TryAccessFIS_Function(System.Action<string, string> func, FIS.FIS_Authorization.ILoginSetting setting)
        {
            try
            {
                FIS.FIS_Authorization form = new FIS.FIS_Authorization(setting);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    func(form.tbLogin.Text, form.tbPassword.Text);
                    return true;
                }
            }
            catch (System.Net.WebException ex)
            {
                if (ShowChoiceMessageBox("Подключён ли компьютер к сети ФИС?", "Ошибка подключения"))
                {
                    MessageBox.Show("Обратитесь к администратору. Не закрывайте это сообщение.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("Информация об ошибке:\n" + ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Выполните подключение.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FIS.FIS_Connector.FIS_Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка ФИС", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
    }
}
