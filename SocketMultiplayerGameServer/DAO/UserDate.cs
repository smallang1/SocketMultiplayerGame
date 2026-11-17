using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; //引入MySQL数据库类
using SocketGameProtocol;

namespace SocketMultiplayerGameServer.DAO
{
    internal class UserDate
    {        
        public bool Logon(MainPack pack, MySqlConnection sqlConnection)
        {
            string username = pack.Loginpack.Username;
            string password = pack.Loginpack.Password;
            try
            {
                string sql = "INSERT INTO `sys`.`userdata`(`username`,`password`) VALUES ('" + username + "','" + password + "')";
                MySqlCommand comd = new MySqlCommand(sql, sqlConnection);

                comd = new MySqlCommand(sql, sqlConnection);

                //ExecuteNonQuery 返回值为int 执行成功为0 回滚，影响数据库返回为-1
                comd.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }      
        }
        public bool Login(MainPack pack,MySqlConnection sqlConnection)
        {
            string username = pack.Loginpack.Username;
            string password = pack.Loginpack.Password;

            string sql = "SELECT * FROM userdata WHERE username='"+ username +"'AND password='" + password + "'";
            MySqlCommand cmd = new MySqlCommand(sql, sqlConnection);
            MySqlDataReader read = cmd.ExecuteReader();
            bool result = read.HasRows;
            read.Close();
            return result;
        }
    }
}
