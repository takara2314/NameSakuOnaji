using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;

namespace NameSakuOnaji {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
		}

		string FileInfoStereotypes = "変換完了：";

		private void DDbox_PreviewDragOver(object sender, DragEventArgs e) {
			e.Effects = DragDropEffects.Copy;
			e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
		}

		async private void DDbox_Drop(object sender, DragEventArgs e) {
			// ドロップされたものが複数な場合は、各ファイルのパス文字列を文字列配列に格納する
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files != null) {
				foreach (string uriString in files) {
					// 絶対パス取得
					string FullPath = System.IO.Path.GetFullPath(uriString);

					// ファイル情報を取得
					FileInfo fi = new FileInfo(FullPath);

					// 拡張子なしのファイル名を取得
					string fileName = System.IO.Path.GetFileNameWithoutExtension(FullPath);

					// 数字以外を除外
					long fileNameDateOnly = long.Parse(Regex.Replace(fileName, @"[^0-9]", ""));

					// 年・月・日・時・分・秒 以外の値をクリア
					if (fileNameDateOnly >= 100000000000000) {
						string fileNameDateOnlyStr = Convert.ToString(fileNameDateOnly);
						fileNameDateOnlyStr = fileNameDateOnlyStr.Remove(14);

						fileNameDateOnly = long.Parse(fileNameDateOnlyStr);
					}

					// 時間を割り出す
					int yearDate = (int)(fileNameDateOnly / 10000000000);
					int monthDate = (int)((fileNameDateOnly % 10000000000) / 100000000);
					int dayDate = (int)((fileNameDateOnly % 100000000) / 1000000);
					int hourDate = (int)((fileNameDateOnly % 1000000) / 10000);
					int minuteDate = (int)((fileNameDateOnly % 10000) / 100);
					int secondDate = (int)(fileNameDateOnly % 100);

					// フォーマットに時間を入れ込む
					string timeDate = "{0}/{1}/{2} {3}:{4}:{5}";
					timeDate = String.Format(timeDate, yearDate, monthDate, dayDate, hourDate, minuteDate, secondDate);

					// 作成日時と更新日時を後進する
					fi.CreationTime = DateTime.Parse(timeDate);
					fi.LastWriteTime = DateTime.Parse(timeDate);

					// 絶対パスを取得してテキストブロックに書き込む
					FileInfoBlock.Text = FileInfoStereotypes + FullPath;
					if (files.Length > 1)
						// 非同期に処理を止める
						await Task.Delay(50);
				}
				MessageBox.Show("すべて完了しました。");
			}
		}
	}
}