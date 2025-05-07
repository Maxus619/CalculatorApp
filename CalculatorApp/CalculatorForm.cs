using System.Globalization;

namespace CalculatorApp;

public partial class CalculatorForm : Form
{
    private readonly TextBox _inputTextBox;
    private readonly Button _calculateButton;
    private readonly Label _resultLabel;
    
    public CalculatorForm()
    {
        Text = "Calculator App";
        Width = 400;
        Height = 150;
        FormBorderStyle = FormBorderStyle.FixedSingle;

        _inputTextBox = new TextBox { Left = 20, Top = 20, Width = 340 };
        _calculateButton = new Button { Text = "Вычислить", Left = 20, Top = 60, Width = 100 };
        _resultLabel = new Label { Left = 140, Top = 64, Width = 220 };

        _calculateButton.Click += CalculateButton_Click;

        Controls.Add(_inputTextBox);
        Controls.Add(_calculateButton);
        Controls.Add(_resultLabel);
    }

    private void CalculateButton_Click(object sender, EventArgs e)
    {
        try
        {
            var expression = _inputTextBox.Text;
            var result = ExpressionEvaluator.Evaluate(expression);
            _resultLabel.Text = result.ToString();
        }
        catch (DivideByZeroException ex)
        {
            MessageBox.Show("Ошибка: Деление на ноль.", "Calculator App Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (FormatException ex)
        {
            MessageBox.Show("Ошибка: Неверный формат.", "CalculatorApp Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "CalculatorApp Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}