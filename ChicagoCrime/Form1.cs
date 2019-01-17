using System;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ChicagoCrime
{
  public partial class Form1 : Form
  {
     public Form1()
    {
      InitializeComponent();
    }

    private bool doesFileExist(string filename)
    {
      if (!System.IO.File.Exists(filename))
      {
        string msg = string.Format("Input file not found: '{0}'",
          filename);

        MessageBox.Show(msg);
        return false;
      }

      // exists!
      return true;
    }

    private void clearForm()
    {
      // 
      // if a chart is being displayed currently, remove it:
      //
      if (this.graphPanel.Controls.Count > 0)
      {
        this.graphPanel.Controls.RemoveAt(0);
        this.graphPanel.Refresh();
      }
    }

    private async void cmdByYear_Click(object sender, EventArgs e)
    {
      string filename = this.txtFilename.Text;

      if (!doesFileExist(filename))
        return;

      clearForm();

      //
      // Call over to F# code to analyze data and return a 
      // chart to display:
      //
      this.Cursor = Cursors.WaitCursor;

      var chart = await Task.Run(() => FSAnalysis.CrimesByYear(filename));

      this.Cursor = Cursors.Default;

      //
      // we have chart, display it:
      //
      this.graphPanel.Controls.Add(chart);
      this.graphPanel.Refresh();
      
    }

    private async void cmdArrest_Click(object sender, EventArgs e)
    {
      string filename = this.txtFilename.Text;

      if (!doesFileExist(filename))
        return;

      clearForm();

      //
      // Call over to F# code to analyze data and return a 
      // chart to display:
      //
      this.Cursor = Cursors.WaitCursor;

      var chart = await Task.Run(() => FSAnalysis.ArrestsByYear(filename));

      this.Cursor = Cursors.Default;

      //
      // we have chart, display it:
      //
      this.graphPanel.Controls.Add(chart);
      this.graphPanel.Refresh();

    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      
    }

    private async void cmdByCrime_Click(object sender, EventArgs e)
    {
      string filename = this.txtFilename.Text;

      if (!doesFileExist(filename))
        return;

      clearForm();

      //
      // Call over to F# code to analyze data and return a 
      // chart to display:
      //
      this.Cursor = Cursors.WaitCursor;

      string codeName = this.txtCode.Text;

      var chart = await Task.Run(() => FSAnalysis.CrimesByIUCR(filename, codeName));

      this.Cursor = Cursors.Default;

      //
      // we have chart, display it:
      //
      this.graphPanel.Controls.Add(chart);
      this.graphPanel.Refresh();

    }

    private async void cmdByArea_Click(object sender, EventArgs e)
    {
      string filename = this.txtFilename.Text;

      if (!doesFileExist(filename))
        return;

      clearForm();

      //
      // Call over to F# code to analyze data and return a 
      // chart to display:
      //
      this.Cursor = Cursors.WaitCursor;

      string name = this.txtArea.Text;

      var chart = await Task.Run(() => FSAnalysis.CrimesByArea(filename, name));

      this.Cursor = Cursors.Default;

      //
      // we have chart, display it:
      //
      this.graphPanel.Controls.Add(chart);
      this.graphPanel.Refresh();

    }

    private void txtArea_TextChanged(object sender, EventArgs e)
    {

    }

    private async void cmdChicago_Click(object sender, EventArgs e)
    {
      string filename = this.txtFilename.Text;

      if (!doesFileExist(filename))
        return;

      clearForm();

      //
      // Call over to F# code to analyze data and return a 
      // chart to display:
      //
      this.Cursor = Cursors.WaitCursor;

      var chart = await Task.Run(() => FSAnalysis.TotalCrimesByArea(filename));

      this.Cursor = Cursors.Default;

      //
      // we have chart, display it:
      //
      this.graphPanel.Controls.Add(chart);
      this.graphPanel.Refresh();
    }
  }//class
}//namespace
