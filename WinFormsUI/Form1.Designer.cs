namespace FolioWebGen.WinForms
{
	partial class MainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowsePanel1 = new FolioWebGen.WinForms.FolderBrowsePanel();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// folderBrowsePanel1
			// 
			this.folderBrowsePanel1.BackColor = System.Drawing.SystemColors.Control;
			this.folderBrowsePanel1.DialogueTitle = "";
			this.folderBrowsePanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.folderBrowsePanel1.Location = new System.Drawing.Point(0, 0);
			this.folderBrowsePanel1.Name = "folderBrowsePanel1";
			this.folderBrowsePanel1.Padding = new System.Windows.Forms.Padding(3);
			this.folderBrowsePanel1.Path = "";
			this.folderBrowsePanel1.Size = new System.Drawing.Size(292, 82);
			this.folderBrowsePanel1.TabIndex = 0;
			this.folderBrowsePanel1.Caption = "Folder";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.folderBrowsePanel1);
			this.Name = "MainWindow";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private FolderBrowsePanel folderBrowsePanel1;
	}
}

