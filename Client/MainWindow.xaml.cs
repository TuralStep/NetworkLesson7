using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client;


public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        _client = new TcpClient("127.0.0.1", 27001);
        _stream = _client.GetStream();
        _bw = new BinaryWriter(_stream);
        _br = new BinaryReader(_stream);

        _buttons = new Button[9]
        {
            btn1, btn2, btn3,
            btn4, btn5, btn6,
            btn7, btn8, btn9
        };


        Task.Run(GetStarted);

    }

    private TcpClient _client;
    private NetworkStream _stream;
    private BinaryReader _br;
    private BinaryWriter _bw;
    private Button[] _buttons;
    private bool _myTurn;
    private char _symbol;

    private void GetChanges()
    {
        while (true)
        {
            Task.Delay(500);

            if (_stream.DataAvailable)
            {
                var chars = _br.ReadChars(9);

                for (int i = 0; i < _buttons.Length; i++)
                    Dispatcher.Invoke(() => _buttons[i].Content = chars[i]);

                _myTurn = !_myTurn;
                Dispatcher.Invoke(() => mainGrid.IsEnabled = _myTurn);
                CheckForWinner(chars);

            }
        }
    }

    private void GetStarted()
    {
        _myTurn = _br.ReadBoolean();
        _symbol = _br.ReadChar();
        Dispatcher.Invoke(() => mainGrid.IsEnabled = _myTurn);
        Task.Run(GetChanges);
    }

    private void CheckForWinner(char[] chars)
    {

        bool[,] xPos = new bool[3, 3]
        {
            {chars[0] == 'X',chars[1] == 'X',chars[2] == 'X'},
            {chars[3] == 'X',chars[4] == 'X',chars[5] == 'X'},
            {chars[6] == 'X',chars[7] == 'X',chars[8] == 'X'}
        };

        if (checkDoubleArr(xPos, 0, false) ||
            checkDoubleArr(xPos, 1, false) ||
            checkDoubleArr(xPos, 2, false) ||
            checkDoubleArr(xPos, 0, true) ||
            checkDoubleArr(xPos, 1, true) ||
            checkDoubleArr(xPos, 2, true) ||
            checkDiaqonalsArr(xPos))
            MessageBox.Show("X wins !");

        changeArr(ref xPos);

        if (checkDoubleArr(xPos, 0, false) ||
            checkDoubleArr(xPos, 1, false) ||
            checkDoubleArr(xPos, 2, false) ||
            checkDoubleArr(xPos, 0, true) ||
            checkDoubleArr(xPos, 1, true) ||
            checkDoubleArr(xPos, 2, true) ||
            checkDiaqonalsArr(xPos))
            MessageBox.Show("O wins !");

    }

    private bool checkDoubleArr(bool[,] bools, int dimension, bool other)
    {
        int length = bools.GetLength(dimension);
        bool isTrue = true;

        for (int i = 0; i < length; i++)
        {
            if (other)
            {
                if (!bools[dimension, i])
                    isTrue = false;
            }
            else
            {
                if (!bools[i, dimension])
                    isTrue = false;
            }
        }

        return isTrue;
    }

    private bool checkDiaqonalsArr(bool[,] bools)
    {
        bool isTrue = false;

        if (bools[0, 0] && bools[1, 1] && bools[2, 2] ||
            bools[0, 2] && bools[1, 1] && bools[2, 0])
            isTrue = true;

        return isTrue;
    }

    private void changeArr(ref bool[,] arr)
    {
        for (int i = 0; i < arr.Rank; i++)
        {
            for (int j = 0; j < arr.Length; j++)
            {
                arr[i, j] = !arr[i, j];
            }
        }
    }

    private void btn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (button.Content is null || (char)button.Content == '\0')
            {
                button.Content = _symbol;
                char[] chars = new char[_buttons.Length];

                for (int i = 0; i < _buttons.Length; i++)
                {
                    if (_buttons[i].Content is char c)
                        chars[i] = c;
                }

                _bw.Write(chars);
                _bw.Flush();
                _myTurn = !_myTurn;
                mainGrid.IsEnabled = _myTurn;
                CheckForWinner(chars);

            }
        }
    }

}
