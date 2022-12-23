using static System.Console;

bool[,] field_is_used = new bool[3, 3]; // масив 3*3 у якому зберігається інформація про те, використане поле чи воно пусте
for (int i = 0; i < 3; i++) // заповнюємо цей масив інформацією про те, що ніяка комірка не була використана
{
    for (int j = 0; j < 3; j++) field_is_used[i, j] = false;
}
int[,] field = new int[3, 3]; // основне поле, у якому проходить гра. (-1) - ще не заповнене, 1 - походив гравець, 0 - походив бот
for (int i = 0; i < 3; i++) // за замовчуванням поле пусте, заповнюэмо
{
    for (int j = 0; j < 3; j++) field[i, j] = -1;
}

int x_player_position = 0; // позиція гравця за Х
int y_player_position = 0; // позиція гравця за У
int x_bot_position = 0; // позиція бота за Х
int y_bot_position = 0; // позиція бота за У

int k = 0; // лічильник, який буде рахувати кількість хрестиків та ноликів для того, щоб бот зміг зробити правильний хід
int k2dif = 0;

bool player_made_step = false; // інформація про те, чи зробив гравець хід
bool bot_made_step = false; // інформація про те, чи зробив бот хід

bool win = false; // флаг для того, щоб зберегти інформацію про те, що гравець переміг
bool lose = false; // флаг для того, щоб зберегти інформацію про те, що гравець програв
bool draw = false;
bool warning = false; // флаг для того, щоб зберегти інформацію про те, що гравець намагається поставити хрестик на зайняте поле


void FieldPrint() // метод, який ми будемо кожного разу визивати для того, щоб оновити поле
{
    Console.Clear(); // стираємо все, що було в консолі, щоб воно не дублювалося
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Щоб пересуватися, використовуйте W,A,S,D");
    Console.WriteLine("Щоб зробити хiд натиснiть P");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine();
    for (int i = 0; i < 3; i++) // перевіряємо кожну комірку поля
    {
        for (int j = 0; j < 3; j++)
        {
            if (i == y_player_position && j == x_player_position && field[i, j] == -1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"+ "); // виводимо "+" щоб показати поточне місцезнаходження курсору нашого гравця
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (field[i, j] == -1) Console.Write("- "); // якщо поле пусте, то виводимо "-"
            else if (i == y_player_position && j == x_player_position && field[i, j] != -1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (field[i, j] == 1) Console.Write("x ");
                if (field[i, j] == 0) Console.Write("o ");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                if (field[i, j] == 1) Console.Write("x ");
                if (field[i, j] == 0) Console.Write("o ");
            } // якщо гравець чи бот вже ходили, то тут буде виводитися 1 та 0
        }
        Console.WriteLine();
    }
    Console.WriteLine();
    if (warning == true) // разом з полем нам на екран виведе попередження, якщо ми будемо намагатися поставити хрестик на зайняте місце
    {
        Console.ForegroundColor = ConsoleColor.Red; // змінюємо колір тексту, щоб надпис був червоний
        Console.WriteLine("Ця клiтинка вже використана!");
        Console.ForegroundColor = ConsoleColor.White; // вертаємо білий колір
    }
    if (win == true) // разом з полем нам на екран виведе повідомлення про перемогу, якщо ми переможемо
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Ви виграли!");
        Console.ForegroundColor = ConsoleColor.White;
    }
    if (lose == true) // разом з полем нам на екран виведе повідомлення про поразку, якщо ми програємо
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ви програли!");
        Console.ForegroundColor = ConsoleColor.White;
    }
    if (draw == true && win == false && lose == false) // разом з полем нам на екран виведе повідомлення про поразку, якщо ми програємо
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Нiчия!");
        Console.ForegroundColor = ConsoleColor.White;
    }
}
bool gameactive = true; // флаг для того, щоб по закінченню гри вона зупинялась ( не спрацював наступний while )

int level_difficulty = -1; // складність бота, спочатку вона не визначена (-1)
while (level_difficulty != 1 && level_difficulty != 2 && level_difficulty != 3) // цикл буде повторюватися до тих пір, поки гравець не вибере складність
{
    Console.Write("Виберiть рiвень складностi бота (1 - легко, 2 - нормально, 3 - важко): ");
    level_difficulty = Convert.ToInt32(Console.ReadLine());
    if (level_difficulty != 1 && level_difficulty != 2 && level_difficulty != 3)
    {
        Console.Clear();
        Console.WriteLine("Помилка!");
    }
}
CursorVisible = false; // прибираємо курсор, щоб не заважав
FieldPrint(); // виводимо поле, яке в нас зараз є
while (gameactive == true)
{
    while (player_made_step == false && win == false && lose == false && draw == false) // якщо гравець досі не походив та гра не закінчена
    {
        var move = Console.ReadKey(true).Key; // зчитуємо натискання клавіші на клавіатурі
        switch (move) // перевіряємо, яка клавіша була натиснута
        {
            case ConsoleKey.D: // якщо це Д, то ми пересуваємо наш курсор вправо, для цього виконуємо наступні дії
                if (x_player_position + 1 <= 2) x_player_position++; // if потрібен для того, щоб не вийти за рамки поля курсором
                FieldPrint(); // знову оновлюємо поле, щоб побачити зміни ( ми побачимо пересування курсору, тобто "+"
                break;
            case ConsoleKey.A: // для кожної клавіші повторюємо те саме
                if (x_player_position - 1 >= 0) x_player_position--;
                FieldPrint();
                break;
            case ConsoleKey.W:
                if (y_player_position - 1 >= 0) y_player_position--;
                FieldPrint();
                break;
            case ConsoleKey.S:
                if (y_player_position + 1 <= 2) y_player_position++;
                FieldPrint();
                break;
            case ConsoleKey.P: // при натисканні цієї клавіші ми ставимо хрестик на місце курсора
                if (field[y_player_position, x_player_position] != 0 && field[y_player_position, x_player_position] != 1) // якщо поле ще не зайняте
                {
                    field[y_player_position, x_player_position] = 1; // ставимо хрестик
                    field_is_used[y_player_position, x_player_position] = true; // запам'ятовуємо, що ця комірка вже зайнята хрестиком, щоб бот не зміг туди ставити
                    player_made_step = true; // гравець зробив хід, теперь while зупиниться, та буде хід бота
                }
                else warning = true; // якщо ми нажали Р и при цьому стояли на зайнятій комірці, то ми фіксуємо це, щоб зробити попередження
                FieldPrint(); // оновлюємо поле, щоб побачити попередження ( воно прописане всередині методу )
                warning = false; // відключаємо попередження щоб воно не заважало
                break;
        }
    }// хід ігрока
    { // перевірка на перемогу
        for (int i = 0; i < 3; i++) // перевіряємо, чи виграли ми ( 3 варіанти по горизонталі )
        {
            if (field[i, 0] == 1 && field[i, 1] == 1 && field[i, 2] == 1) // якщо всі хрестики, тобто одинички
            {
                win = true; // фіксуємо перемогу
                gameactive = false; // закінчуємо гру
                x_player_position = -1; // прибираємо гравця з поля, щоб можна було побачити всі хрестики та нолики ( інакше замість якогось буде стояти "+")
                y_player_position = -1;
                FieldPrint(); // оновлюємо поле, щоб побачити результат
            }
        }
        for (int j = 0; j < 3; j++) // перевіряємо, чи виграли ми ( 3 варіанти по вертикалі )
        {
            if (field[0, j] == 1 && field[1, j] == 1 && field[2, j] == 1)
            {
                win = true;
                gameactive = false;
                x_player_position = -1;
                y_player_position = -1;
                FieldPrint();
            }
        }
        if ((field[0, 0] == 1 && field[1, 1] == 1 && field[2, 2] == 1) || (field[0, 2] == 1 && field[1, 1] == 1 && field[2, 0] == 1)) // перевіряємо, чи виграли ми ( залишилися тільки діагоналі )
        {
            win = true;
            gameactive = false;
            x_player_position = -1;
            y_player_position = -1;
            FieldPrint();
        }
    }// перевірка на перемогу
    { // перевірка на нічию
        int nused = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++) if (field_is_used[i, j] == true) nused++;
        }
        if (nused == 9)
        {
            draw = true;
            gameactive = false;
            x_player_position = -1;
            y_player_position = -1;
            FieldPrint();
        }
    } // перевірка на нічию
    player_made_step = false; // це для того, щоб після того, як походить бот, нам дозволили знову зробити хід ( запрацював while )
    while (bot_made_step == false && win == false && lose == false && draw == false) // доки бот не походить, цикл буде повторюватися
    {
        if (level_difficulty == 3 || (level_difficulty == 2 && k2dif == 0)) // якщо 2, то завдяки k2dif бот буде думати через раз, а в іншому випадку буде ставити не рандом
        {

            // перевірки для того, щоб бот походив "по-умному"
            for (int i = 0; i < 3; i++) // два нуля по горизонтали
            {
                if (bot_made_step == false)
                {
                    k = 0;
                    for (int j = 0; j < 3; j++) if (field[i, j] == 0) k++;
                    if (k == 2)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (field_is_used[i, j] == false)
                            {
                                field[i, j] = 0;
                                field_is_used[i, j] = true;
                                bot_made_step = true;
                                FieldPrint();
                            }
                        }
                    }
                }
            } // два нуля по горизонтали

            for (int j = 0; j < 3; j++) // два нуля по вертикали
            {
                if (bot_made_step == false)
                {
                    k = 0;
                    for (int i = 0; i < 3; i++) if (field[i, j] == 0) k++;
                    if (k == 2)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (field_is_used[i, j] == false)
                            {
                                field[i, j] = 0;
                                field_is_used[i, j] = true;
                                bot_made_step = true;
                                FieldPrint();
                            }
                        }
                    }
                }
            } // два нуля по вертикали
            if (bot_made_step == false) // два нуля на головній діагоналі
            {
                k = 0;
                if (field[0, 0] == 0) k++;
                if (field[1, 1] == 0) k++;
                if (field[2, 2] == 0) k++;
                if (k == 2)
                {
                    if (field_is_used[0, 0] == false)
                    {
                        field[0, 0] = 0;
                        field_is_used[0, 0] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[1, 1] == false)
                    {
                        field[1, 1] = 0;
                        field_is_used[1, 1] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[2, 2] == false)
                    {
                        field[2, 2] = 0;
                        field_is_used[2, 2] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                }
            } // два нуля на головній діагоналі

            if (bot_made_step == false) // два нуля на побічній діагоналі
            {
                k = 0;
                if (field[0, 2] == 0) k++;
                if (field[1, 1] == 0) k++;
                if (field[2, 0] == 0) k++;
                if (k == 2)
                {
                    if (field_is_used[0, 2] == false)
                    {
                        field[0, 2] = 0;
                        field_is_used[0, 2] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[1, 1] == false)
                    {
                        field[1, 1] = 0;
                        field_is_used[1, 1] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[2, 0] == false)
                    {
                        field[2, 0] = 0;
                        field_is_used[2, 0] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                }
            } // два нуля на побічній діагоналі


            for (int i = 0; i < 3; i++) // дві одиниці по горизонтали
            {
                if (bot_made_step == false)
                {
                    k = 0;
                    for (int j = 0; j < 3; j++) if (field[i, j] == 1) k++;
                    if (k == 2)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (field_is_used[i, j] == false)
                            {
                                field[i, j] = 0;
                                field_is_used[i, j] = true;
                                bot_made_step = true;
                                FieldPrint();
                            }
                        }
                    }
                }
            } // дві одиниці по горизонтали

            for (int j = 0; j < 3; j++) // дві одиниці по вертикали
            {
                if (bot_made_step == false)
                {
                    k = 0;
                    for (int i = 0; i < 3; i++) if (field[i, j] == 1) k++;
                    if (k == 2)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (field_is_used[i, j] == false)
                            {
                                field[i, j] = 0;
                                field_is_used[i, j] = true;
                                bot_made_step = true;
                                FieldPrint();
                            }
                        }
                    }
                }
            } // дві одиниці по вертикали
            if (bot_made_step == false) // дві одиниці на головній діагоналі
            {
                k = 0;
                if (field[0, 0] == 1) k++;
                if (field[1, 1] == 1) k++;
                if (field[2, 2] == 1) k++;
                if (k == 2)
                {
                    if (field_is_used[0, 0] == false)
                    {
                        field[0, 0] = 0;
                        field_is_used[0, 0] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[1, 1] == false)
                    {
                        field[1, 1] = 0;
                        field_is_used[1, 1] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[2, 2] == false)
                    {
                        field[2, 2] = 0;
                        field_is_used[2, 2] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                }
            } // дві одиниці на головній діагоналі

            if (bot_made_step == false) // дві одиниці на побічній діагоналі
            {
                k = 0;
                if (field[0, 2] == 1) k++;
                if (field[1, 1] == 1) k++;
                if (field[2, 0] == 1) k++;
                if (k == 2)
                {
                    if (field_is_used[0, 2] == false)
                    {
                        field[0, 2] = 0;
                        field_is_used[0, 2] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[1, 1] == false)
                    {
                        field[1, 1] = 0;
                        field_is_used[1, 1] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                    if (field_is_used[2, 0] == false)
                    {
                        field[2, 0] = 0;
                        field_is_used[2, 0] = true;
                        FieldPrint();
                        bot_made_step = true;
                    }
                }
            } // дві одиниці на побічній діагоналі




            k2dif++; // для того щоб бот в наступний раз не думав, а ходив на рандом

        }
        if (bot_made_step == false)
        {
            Random rnd = new Random();
            x_bot_position = rnd.Next(0, 3); // беремо випадкву комірку в полі по Х та У
            y_bot_position = rnd.Next(0, 3);
            if (field_is_used[y_bot_position, x_bot_position] == false) // при умові, що вона не зайнята
            {
                field[y_bot_position, x_bot_position] = 0; // бот ставить нолик
                field_is_used[y_bot_position, x_bot_position] = true; // фіксуємо, що комірка зайнята
                bot_made_step = true; // запам'ятовуємо, що бот зробив крок
                FieldPrint(); // оновлюємо поле
            }
            if (k2dif == 1) k2dif--;
        }
    }// хід бота
    bot_made_step = false; // це для того, щоб після того, як походить гравець знову походить, бот зміг знову зробити хід ( запрацював while )
    { // перевірка на програш
        for (int i = 0; i < 3; i++) // перевіряємо, чи ми програли після того, як походив бот ( та сама перевірка, що для перемоги, але тут нулі )
        {
            if (field[i, 0] == 0 && field[i, 1] == 0 && field[i, 2] == 0)
            {
                lose = true;
                gameactive = false;
                x_player_position = -1;
                y_player_position = -1;
                FieldPrint();
            }
        }
        for (int j = 0; j < 3; j++)
        {
            if (field[0, j] == 0 && field[1, j] == 0 && field[2, j] == 0)
            {
                lose = true;
                gameactive = false;
                x_player_position = -1;
                y_player_position = -1;
                FieldPrint();
            }
        }
        if ((field[0, 0] == 0 && field[1, 1] == 0 && field[2, 2] == 0) || (field[0, 2] == 0 && field[1, 1] == 0 && field[2, 0] == 0))
        {
            lose = true;
            gameactive = false;
            x_player_position = -1;
            y_player_position = -1;
            FieldPrint();
        }
    }// перевірка на програш


}