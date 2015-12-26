using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eBdb.EpubReader;
using Ionic.Zip;
using System.IO;
using DevComponents.DotNetBar;

namespace SimpeEpubReader
{
	public partial class MainForm : DevComponents.DotNetBar.Metro.MetroForm
	{
		protected internal String fileLocation;

		protected internal Epub book;
		protected internal String bookName;
		protected internal String author;

		protected internal List<NavPoint> navPoints;
		protected internal Dictionary<String, String> book_content = new Dictionary<String, String>();

		public MainForm()
		{
			InitializeComponent();

			TitleText = "<div align=\"left\">  " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "</div>";
			MaximizeBox = false;
			SettingsButtonText = "<b>OPEN EPUB</b>";
			SettingsButtonVisible = true;
			SettingsButtonClick += FormSettingsButtonClick;
			/*HelpButtonText = LANG.getString("mainpage_about");
			HelpButtonVisible = true;
			HelpButtonClick += FormHelpButtonClick;
			HelpButton = true;
			HelpButtonClicked += FormHelpButtonClicked;*/

			labelX1.Text = "目录";
			treeView1.BorderStyle = BorderStyle.None;
			treeView1.NodeMouseClick += treeView1_NodeMouseClick;
			treeView1.KeyUp += treeView1_KeyUp;

			webBrowser1.PreviewKeyDown += new PreviewKeyDownEventHandler(webBrowser1_PreviewKeyDown);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			splitContainer1.BackColor = styleManager1.MetroColorParameters.BaseColor;
			panel1.BackColor = Color.White;
			panel2.BackColor = Color.White;
		}

		private void FormSettingsButtonClick(object sender, EventArgs e)
		{
			openFileDialog1.Title = "Select EPUB";
			openFileDialog1.Filter = "EPUB|*.epub";
			if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
			{
				if (treeView1.Nodes != null && treeView1.Nodes.Count != 0)
					treeView1.Nodes.Clear();
				if (book_content != null && book_content.Count != 0)
					book_content.Clear();
				if (navPoints != null && navPoints.Count != 0)
					navPoints.Clear();


				fileLocation = openFileDialog1.FileName;

				book = new Epub(fileLocation);
				bookName = book.Title[0];
				author = book.Creator[0];
				navPoints = book.TOC;


				for (Int32 i = 0; i < navPoints.Count; i++)
				{
					NavPoint n = navPoints[i];
					TreeNode curNode = treeView1.Nodes.Add(n.Title);
					try
					{
						book_content.Add(curNode.FullPath, n.ContentData.Content);
					}
					catch {	}
					PopulateTreeView(n, curNode);
				}

				selectNode(treeView1.Nodes[0]);

			}
		}

		private void PopulateTreeView(NavPoint nav, TreeNode parentNode)
		{
			if (nav.Children != null && nav.Children.Count != 0)
			{
				for (Int32 i = 0; i < nav.Children.Count; i++)
				{
					NavPoint n = nav.Children[i];
					TreeNode curNode = parentNode.Nodes.Add(n.Title);
					try
					{
						book_content.Add(curNode.FullPath, n.ContentData.Content);
					}
					catch {	}
					PopulateTreeView(n, curNode);
				}
			}
			
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			selectNode(e.Node);
		}

		private void selectNode(TreeNode node)
		{
			if (node != null)
			{
				treeView1.SelectedNode = node;
				try
				{
					webBrowser1.DocumentText = book.GetChapterContentAsHtml(node.Text, book_content[node.FullPath]);
				}
				catch
				{
					webBrowser1.DocumentText = ERRORPAGE;
				}
			}
		}

		private void treeView1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
			{
				selectNode(treeView1.SelectedNode);
			}
			else if (e.KeyCode == Keys.Enter)
			{
				selectNode(treeView1.SelectedNode);
				if (treeView1.SelectedNode.IsExpanded)
					treeView1.SelectedNode.Collapse();
				else
					treeView1.SelectedNode.Expand();
			}
		}

		private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
					if (treeView1.SelectedNode != null)
					{
						selectNode(treeView1.SelectedNode.RealPrevNode());
					}
					return;
				case Keys.Right:
					if (treeView1.SelectedNode != null)
					{
						selectNode(treeView1.SelectedNode.RealNextNode());
					}
					return;
			}
		}

		

		private static String ERRORPAGE = "<!DOCTYPE html PUBLIC \"\"-//W3C//DTD XHTML 1.1//EN\"\" \"\"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\"\">\n<html>\n<head>\n<title>Invalid Page</title>\n<style type=\"text/css\">\np{\ncolor:black;\ntext-align:center;\nfont-size:small;\n}\nh1{\ncolor:rgb(40,120,240);\ntext-align:center;\nmargin-top:2em;\nmargin-bottom:2em\n}\n</style>\n</head>\n<body>\n<h1><span style=\'font-size:48pt;font-family:\"Calibri\",sans-serif;\'>This page is invalid</span></h1>\n<p></p>\n<p><i>Something is wrong with this page. Please double check.</i></p>\n</body>\n</html>";
	}
}
