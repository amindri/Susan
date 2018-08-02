using System;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.IO;
using System.Collections.Generic;
using log4net;
using System.Reflection;

namespace FirstInFirstAid.Helpers
{
    public class BackupManager
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string INSTANCE = "SQLEXPRESS"; //SQL SERVER name
        private static string DATABASE = "FirstInFirstAid"; // Database name
        private ServerConnection conn;    

        //Creates a connection to database
        private ServerConnection CreateConnection()
        {
            if (conn == null || !conn.IsOpen)
            {
                conn = new ServerConnection
                {
                    LoginSecure = true,
                    DatabaseName = DATABASE,
                    ServerInstance = @".\" + INSTANCE
                };
                conn.Connect();
            }            
            return conn;         
        }

        //Disconnects the connection
        private void CloseConnection()
        {
            if (conn != null) {
                conn.Disconnect();
            }
        }

        //retrieves the back folder of the given SQL server instance
        public string GetBackupFolder()
        {
            CreateConnection();
            Server server = new Server(conn);
            string path = server.Settings.BackupDirectory;
            CloseConnection();
            return path;
        }
      

        //Creates a backup of the database specified by DATABASE var, and saves the back in the SQL server back folder
        public void CreateBackup()
        {
            try
            {
                CreateConnection();
                if (conn != null)
                {
                    Server server = new Server(conn);
                    Database database = server.Databases[DATABASE];
                    string backupFolder = server.Settings.BackupDirectory;

                    if (database.IsSystemObject == false && database.IsMirroringEnabled == false)
                    {
                        DateTime datNow = DateTime.Now;
                        string strDBName = database.Name;
                        string strBKDate = datNow.ToString("yyyyMMddhhmmss");
                        string backupName = strDBName + "_db_" + strBKDate + ".bak";

                        Backup objBackup = new Backup
                        {
                            Action = BackupActionType.Database,
                            Database = strDBName,
                            BackupSetDescription = "Full backup of " + strDBName,
                            BackupSetName = strDBName + " Backup",
                            MediaDescription = "Disk"
                        };

                        objBackup.Devices.AddDevice(backupFolder + "\\" + backupName, DeviceType.File);
                        objBackup.SqlBackup(server);

                        //Creates a translog backup 
                        string translogBkupName = "";
                        if (database.DatabaseOptions.RecoveryModel != RecoveryModel.Simple)
                        {
                            datNow = DateTime.Now;
                            string strTBKDate = datNow.ToString("yyyyMMddhhmmss");
                            Backup objTrnBackup = new Backup
                            {
                                Action = BackupActionType.Log,
                                BackupSetDescription = "Trans Log backup of " + strDBName,
                                BackupSetName = strDBName + " Backup",
                                Database = strDBName,
                                MediaDescription = "Disk"
                            };
                            translogBkupName = strDBName + "_tlog_" + strTBKDate + ".trn";
                            objTrnBackup.Devices.AddDevice(backupFolder + "\\" + translogBkupName, DeviceType.File);
                            objTrnBackup.SqlBackup(server);
                        }
                        DeleteOtherBackups(backupFolder, backupName, translogBkupName);
                    }
                }
            }           
            finally
            {
                CloseConnection();
            }
        }

        //deletes all the backups in the SQL server backup folder except for the backup specified by name "newbackup"
        private void DeleteOtherBackups(String backupFolder, string newbackup, string translogBkupName)
        {
            var dir = new DirectoryInfo(backupFolder);
            FileInfo[] fileNames = dir.GetFiles("*.*");
            List<string> items = new List<string>();
            foreach (var file in fileNames)
            {
                if (!file.Name.Equals(newbackup) && !file.Name.Equals(translogBkupName)) {
                    file.Delete();
                }
            }
        }

        //Restores the database using the file resides in the path specified by "backupFilePath" 
        public void Restore(string backupFilePath)
        {          
            try
            {
                CreateConnection();
                Server svr = new Server(conn);
                Database db = svr.Databases[DATABASE];
            
                Restore restore = new Restore
                {
                    Database = DATABASE,
                    RestrictedUser = true,
                    Action = RestoreActionType.Database,
                    ReplaceDatabase = true
                };

                restore.Devices.AddDevice(backupFilePath, DeviceType.File);
                svr.KillAllProcesses(DATABASE);
                restore.Wait();
                restore.SqlRestore(svr);
            }           
            finally
            {
                CloseConnection();
            }
        }

        /**
         * Currently the application doesn't have any other file upload actions other than backup upload. 
         * Therefore it is safe to delete all the files in the uploads directory
         **/
        public void CleanUploadsDir(string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);
            foreach (FileInfo file in dir.EnumerateFiles())
            {
                file.Delete();
            }           
        }
    }

    
}