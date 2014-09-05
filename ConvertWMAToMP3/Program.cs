/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2013/11/29
 * Time: 14:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ConvertWMAToMP3
{
	class Program
	{
		//F:\music\mp3
		static string dirPath="";
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			
			Console.Write("Press any key to continue . . . \n");
			Console.ReadKey(true);
			Console.Write("Convert is Begin...."+DateTime.Now.ToShortTimeString()+"\n\n");
			f:
			Console.Write("\n******************************************\n\n\n请输入文件存在的文件夹路径(根目录):");
			string filepath=Console.ReadLine();
			Console.Write("您输入的文件路径为:"+filepath+"\n 如果确认您输入:Y ，重新开始输入:R");
			string line=Console.ReadLine();
			if(line=="Y"||line=="y")
			{
				dirPath=filepath;
				convert();
				
			}
			else if(line=="R"||line=="r")
			{
				goto f;
			}
			else
			{
				Console.Clear();
				Console.Write("输入有误....\n");
				goto f;
			}
			
		}
		private static void convert()
		{
			if(dirPath==null||dirPath.Trim().Length==0)
			{
				Console.Write("文件路径错误....重新启动程序....\n");
				Console.ReadKey(true);
			
			}
			else
			{
				if(!Directory.Exists(dirPath))
				{
					Console.Write("文件路径错误....重新启动程序....\n");
					Console.ReadKey(true);
					return;
				}
				else
				{
					try{
						Console.Write("加载中....\n\n\n");
						DirectoryInfo info=new DirectoryInfo(dirPath);
					
						FileInfo []files=info.GetFiles("*.wma",SearchOption.AllDirectories);
						Console.Write("总的文件数量:"+files.Length.ToString()+"....\n");
						foreach(FileInfo file in files)
						{
							
							string filePath=file.FullName;
							string dirName=file.Directory.FullName+@"\";
							string fileName=file.Name.Substring(0,file.Name.LastIndexOf("."));
							string newFileName=dirName+fileName+".mp3";
							Console.WriteLine(newFileName);
							Console.WriteLine("正在转换:"+filePath);
							ConvertVideo(filePath,newFileName);
						}
						Console.ReadKey(true);
						
					}
					catch(Exception ex)
					{
						Console.Write("error:\n"+ex.Message);
						Console.ReadKey(true);
					}
				}
			}
		}//end function
		
		public static void ConvertVideo(string path1, string path2)
		{
			try
			{
				if(File.Exists(path2))
				{
					Console.WriteLine("文件已存在..."+path2);
					return ;
				}
				Process p = new Process();//建立外部调用线程
				p.StartInfo.FileName = "ffmpeg.exe";//要调用外部程序的绝对路径
				//  p.StartInfo.Arguments = " -i " + path1 + " -ab 32  -qscale 6 -r 25 -s 240x170 " + path2;//参数(这里就是FFMPEG的参数了)
				//ffmpeg -i input.m4a -acodec libmp3lame -ab 128k output.mp3
				p.StartInfo.Arguments = " -i \"" + path1 + "\" -y -acodec libmp3lame -ab 64k \"" + path2+"\"";
				//  p.StartInfo.Arguments = " -i " + path1 + " -f psp -r 29.97 -b 768k -ar 24000 -ab 32k -s 320x240 " + path2;//参数(这里就是FFMPEG的参数了)
				p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
				p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
				p.StartInfo.CreateNoWindow = true;//不创建进程窗口
				p.ErrorDataReceived += new DataReceivedEventHandler(Output);//外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
				p.Start();//启动线程
				p.BeginErrorReadLine();//开始异步读取
				p.WaitForExit();//阻塞等待进程结束
				p.Close();//关闭进程
				p.Dispose();//释放资源
			}
			catch (Exception ex)
			{
				using(StreamWriter swerror=new StreamWriter("error.log",true,Encoding.UTF8))
				{
					string errortime=DateTime.Now.ToString();
					swerror.WriteLine("-------------------------------");
					swerror.WriteLine("转换文件:errortime="+errortime+"\n error="+ex.Message);
					
				}
			}

		}
		private static  void Output(object sendProcess, DataReceivedEventArgs output)
		{
//			
//			using(StreamWriter swerror=new StreamWriter("error.log",true,Encoding.UTF8))
//			{
//				string errortime=DateTime.Now.ToString();
//				//	swerror.WriteLine("-------------------------------");
//				//	swerror.WriteLine("转换文件:="+errortime+"\n info="+output.Data);
//				
//			}
			Console.WriteLine(output.Data);
			return;
		}
	}
}