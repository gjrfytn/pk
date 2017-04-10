using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Classes
{
    static class Utility
    {
        public static readonly string DocumentsTemplatesPath = System.Configuration.ConfigurationManager.AppSettings["DocumentsTemplatesPath"];
        public static readonly string FIS_Login = System.Configuration.ConfigurationManager.AppSettings["FIS_Login"];

        /// <summary>
        /// Отображает диалоговое окно с заголовком "Действие" и кнопками "Да" и "Нет".
        /// </summary>
        /// <param name="description">Текст сообщения.</param>
        /// <returns><c>true</c>, если нажата кнопка "Да".</returns>
        public static bool ShowActionMessageBox(string description)
        {
            return MessageBox.Show(
                description,
                "Действие",
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
        /// Вызывает <see cref="ShowActionMessageBox(string)"/>. Далее, если нажата кнопка "Да", - <see cref="ShowUnrevertableActionMessageBox"/>.
        /// Если во втором окне нажата конпка "Нет", то снова вызывается первое.
        /// </summary>
        /// <param name="description">Текст сообщения для первого окна.</param>
        /// <returns><c>true</c>, если в обоих окнах нажата кнопка "Да".</returns>
        public static bool ShowActionMessageWithConfirmation(string description)
        {
            while (true)
                if (ShowActionMessageBox(description))
                {
                    if (ShowUnrevertableActionMessageBox())
                        return true;
                }
                else
                    return false;
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

            rooms = rooms.OrderByDescending(r => r.Value).ToDictionary(k => k.Key, v => v.Value);
            letters = letters.OrderByDescending(l => l.Value).ToDictionary(k => k.Key, v => v.Value);
            //rooms.OrderByDescending(r => r.Value).ToDictionary(k => k.Key, v => v.Value);
            // letters.OrderByDescending(l => l.Value).ToDictionary(k => k.Key, v => v.Value);
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
                        if (roomBuf != null)
                            break;
                        else
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
                    }
                    else
                    {
                        roomBuf = room.Key;

                        if (lSum == room.Value)
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
                // letters.OrderByDescending(a => a.Value).ToDictionary(k => k.Key, v => v.Value);
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
    }
}
