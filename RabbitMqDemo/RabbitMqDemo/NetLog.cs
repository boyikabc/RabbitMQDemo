using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Data.SqlClient;

/// <summary>
/// Summary description for NetLog
/// </summary>
/// 

public enum NetLogType{
     Info = 0
}

public class NetLog
{
    /// <summary>
    /// 写入日志到文本文件
    /// </summary>
    /// <param name="action">动作</param>
    /// <param name="strMessage">日志内容</param>
    /// <param name="time">时间</param>
    private static void WriteTextLog(string flag ,string action, string strMessage, DateTime time)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + @"\log\";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string fileFullPath = path + time.ToString("yyyy-MM-dd") + ".System.txt";

        if(flag=="error"){
            fileFullPath = path + time.ToString("yyyy-MM-dd") + ".error.txt";
        }

        StringBuilder str = new StringBuilder();
        str.Append("Action:" + action + " \t Time:" + time.ToString() + "\r\n");
        str.Append("Message:" + strMessage + "\r\n");
        StreamWriter sw;
        if (!File.Exists(fileFullPath))
        {
            sw = File.CreateText(fileFullPath);
        }
        else
        {
            sw = File.AppendText(fileFullPath);
        }
        sw.WriteLine(str.ToString());
        sw.Close();
    }
    /// <summary>
    /// 写日至
    /// </summary>
    /// <param name="action"></param>
    /// <param name="strMessage"></param>
    public static void WriteTextLog(string action, string strMessage)
    {
        try
        {
            WriteTextLog("",action, strMessage, DateTime.Now);
        }
        catch (Exception e)
        {
            Console.WriteLine("记录日志失败："+e.Message);
        }
    }

    public static void WriteTextErrorLog(string action, string strMessage)
    {
        try
        {
            WriteTextLog("error", action, strMessage, DateTime.Now);
        }
        catch (Exception e)
        {

        }
    }

    public static void insertLog(SqlConnection conn,string userId, string userName, System.Reflection.MethodBase methodBase,
        string title, string inMsg, string outMsg)
    {
        try
        {
            try
            {
                string method = "";
                if (methodBase != null)
                {
                    //method = methodBase.ReflectedType.Name + "." + methodBase.Name;
                    method = methodBase.ReflectedType.FullName + "." + methodBase.Name;
                }
                string sql = "INSERT INTO NetLog(type,loginUserId,loginUserName,method,title,inMsg,outMsg) VALUES(@type,@loginUserId,@loginUserName,@method,@title,@inMsg,@outMsg)";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@type", (int)NetLogType.Info);
                cmd.Parameters.AddWithValue("@loginUserId", userId == null ? "" : userId);
                cmd.Parameters.AddWithValue("@loginUserName", userName == null ? "" : userName);
                cmd.Parameters.AddWithValue("@method", method);
                cmd.Parameters.AddWithValue("@title", title == null ? "" : title);
                cmd.Parameters.AddWithValue("@inMsg", inMsg == null ? "" : inMsg);
                cmd.Parameters.AddWithValue("@outMsg", outMsg == null ? "" : outMsg);
                cmd.CommandText = sql;//设置SQL语句 
                cmd.Connection = conn;//调用打开数据库连接方法 
                cmd.ExecuteNonQuery();//执行
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
        catch (Exception e)
        {
            NetLog.WriteTextLog("插入日至异常", e.Message);
        }
    }

}