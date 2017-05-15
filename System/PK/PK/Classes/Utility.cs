using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    static class Utility
    {

        public const string TempPath = ".\\temp\\";

        public static readonly string DocumentsTemplatesPath = Properties.Settings.Default.DocumentsTemplatesPath;

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
        /// Отображает диалоговое окно, предупреждающее о потере несохранённых данных, с кнопками "Да" и "Нет".
        /// </summary>
        /// <returns><c>true</c>, если нажата кнопка "Да".</returns>
        public static bool ShowChangesLossMessageBox()
        {
            return MessageBox.Show(
                  "Все несохранённые данные будут потеряны. Вы уверены?",
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
            if (rooms.Sum(r => r.Value) < letters.Sum(l => l.Value))
                throw new System.ArgumentException("Общее количество мест в аудиториях меньше общего количества абитуриентов.");

            letters = letters.OrderByDescending(l => l.Value).ToDictionary(k => k.Key, v => v.Value);
            List<System.Tuple<char, string>> distributions = new List<System.Tuple<char, string>>();
            while (letters.Count != 0)
            {
                Dictionary<char, ushort> excludedLetters = new Dictionary<char, ushort>();
                Dictionary<char, byte> splitLetterCounts = new Dictionary<char, byte>();
                string roomBuf = null;
                foreach (var room in rooms)
                {
                    ifLabel:
                    ushort lSum = (ushort)letters.Sum(l => l.Value);
                    if (lSum > room.Value)
                    {
                        char letter = letters.Last().Key;
                        if (letters.Count == 1)
                        {
                            if (!splitLetterCounts.ContainsKey(letter))
                                splitLetterCounts.Add(letter, 0);
                            splitLetterCounts[letter]++;

                            excludedLetters.Add(letter, (ushort)(letters[letter] - room.Value));
                            letters[letter] = room.Value;
                        }
                        else
                        {
                            excludedLetters.Add(letter, letters[letter]);
                            letters.Remove(letter);
                        }

                        goto ifLabel; //TODO ?
                    }
                    else
                    {
                        roomBuf = room.Key;
                        break;
                    }
                }

                rooms.Remove(roomBuf);

                foreach (var letter in letters)
                    distributions.Add(new System.Tuple<char, string>(letter.Key, roomBuf));

                letters.Clear();

                foreach (var el in excludedLetters)
                    letters.Add(el.Key, el.Value);
                letters = letters.OrderByDescending(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
            }

            return distributions;
        }

        public static void Print(string file)
        {
            // System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(file);
            // info.Verb = "Print";
            // info.CreateNoWindow = true;
            // info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            // System.Diagnostics.Process.Start(info);
            System.Diagnostics.Process.Start(file);

            //p.WaitForExit();надо?
            //p.Close();?
            //p.Dispose();?
        }

        public static bool GetFIS_AuthData(out string login, out string password)
        {
            Forms.FIS_Authorization form = new Forms.FIS_Authorization();
            if (form.ShowDialog() == DialogResult.OK)
            {
                login = form.tbLogin.Text;
                password = form.tbPassword.Text;
                return true;
            }
            else
            {
                login = null;
                password = null;
                return false;
            }
        }
    }
}
