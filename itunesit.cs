using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace iTunesIt
{
    class itunesit
    {
        public static itunesit instances = null;
        
        /*
         *  Необходимые нам файлы
         */
        private string[] itunes_library_files = {"iTunes Music Library.xml", "iTunes Library.itl"};
        private string itunes_directory = "iTunes";

        /*
         *  Наша библиотека itunes
         */
        private string my_itunes_library_path = null;

        /*
         *  Локальная библиотека itunes
         */
        private string local_itunes_library_path = null;

        private string temp_dir = null;
        private string config_path = "config/config.ini";
        private string errors = null;

        /*
         *  Вернули ли мы всё на место?
         */
        private bool local_files_returned = true;

        public itunesit()
        {
            this.init();
        }

        /*
         *  Синглтон
         */
        public static itunesit instance()
        {
            if (itunesit.instances == null)
            {
                itunesit.instances = new itunesit();
            }
            return itunesit.instances;
        }

        /*
         *  Инициализируем папки и пути
         *  Псевдокоснтруктор
         */
        public bool init()
        {
            this.init_temp_dir();
            return this.init_itunes_library_path();
        }

        /*
         *  Функция сохраняет файл this.local_itunes_library_path в папку с бекапами
         */
        public bool backup_local_library()
        {
            foreach (string filename in this.itunes_library_files)
            {
                string filepath = this.local_itunes_library_path + filename;
                string tmp_file = this.temp_dir + Path.DirectorySeparatorChar + filename;

                this.copy_file(filepath, tmp_file);
            }

            return true;
        }

        private bool copy_file(string from, string to)
        {
            FileInfo file = new FileInfo(from);
            try
            {
                File.Copy(from, to, true);
            }
            catch (FileNotFoundException e)
            {
                this.errors = e.Message;
                return false;
            }
            return true;
        }

        /*
         *  Заменяет файлы this.local_itunes_library на this.my_itunes_library
         *  (подменим локальные файлы на наши)
         */
        public bool replace_itunes_library()
        {
            this.backup_local_library();

            foreach (string filename in this.itunes_library_files)
            {
                string from = this.my_itunes_library_path + filename;
                string to = this.local_itunes_library_path + filename;

                this.copy_file(from, to);
            }

            this.local_files_returned = false;
            return true;
        }
        
        /*
         *  Возвращяет this.local_itunes_library на место
         */
        public bool return_local_library()
        {
            /*
             *  Сохраним новую библиотеку
             */
            this.save_my_itunes_library();

            foreach (string filename in this.itunes_library_files)
            {
                string from = this.temp_dir + filename;
                string to = this.local_itunes_library_path + filename;

                this.copy_file(from, to);
            }
            this.local_files_returned = true;
            return true;
        }

        /*
         *  Сохраним новую библиотеку
         */
        public void save_my_itunes_library()
        {
            foreach (string filename in this.itunes_library_files)
            {
                string from = this.local_itunes_library_path + filename;
                string to = this.my_itunes_library_path + filename;

                this.copy_file(from, to);
            }
        }

        public void synchronize_my_library()
        {
            this.replace_itunes_library();
            this.local_files_returned = true;
        }

        /*
         *  Возвращает ошибки
         */
        public string get_errors()
        {
            return this.errors;
        }

        private bool init_itunes_library_path()
        {
            string[] variables = { "HOME", "USERPROFILE" };
            string home_path = null;

            foreach (string variable in variables)
            {
                home_path = Environment.GetEnvironmentVariable(variable);
                if (home_path != null)
                {
                    break;
                }
            }

            if (home_path == null)
            {
                this.errors = "Не могу найти путь к папке пользователя";
                return false;
            }

            char separator = Path.DirectorySeparatorChar;

            this.my_itunes_library_path = "my_library" + separator;

            /*
             *  TODO: Выпилить это
             */
            string my_music_dir = "Music";
            string itunes_library_path = home_path + separator + my_music_dir + separator + this.itunes_directory + separator;

            foreach (string filename in this.itunes_library_files)
            {
                string filepath = itunes_library_path + filename;
                FileInfo file = new FileInfo(filepath);

                if (false == file.Exists)
                {
                    this.errors += "Файл " + filename + " не найден";
                    return false;
                }
            }

            /*
             *  Нужные файлы на нужном месте
             */
            this.local_itunes_library_path = itunes_library_path;
            return true;
        }

        /*
         *  Устанавливает путь до папки библиотеки iTunes, и тут же забекапит нужные файлы
         */
        public void set_local_itunes_library_path(string path)
        {
            this.local_itunes_library_path = path;
            this.backup_local_library();
        }

        /*
         *  Инициализируем временную папку
         */
        private void init_temp_dir()
        {
            this.temp_dir = "temp" + Path.DirectorySeparatorChar;

            DirectoryInfo dir = new DirectoryInfo(this.temp_dir);

            if (false == dir.Exists)
            {
                dir.Create();
            }
        }

        /*
         *  Очищает временую папку
         */
        private void clear_temp_dir()
        {
            DirectoryInfo tmp_dir = new DirectoryInfo(this.temp_dir);
            FileInfo[] files;
            files = tmp_dir.GetFiles();

            foreach (FileInfo file in files)
            {
                file.Delete();
            }
        }

        public bool get_local_files_returned()
        {
            return this.local_files_returned;
        }

        public bool get_local_itunes_library_path()
        {
            return this.local_itunes_library_path != null;
        }

        ~itunesit()
        {
            /*
             *  Если мы еще не вернули всё на место, то нужно вернуть !
             */
            if (false == this.local_files_returned)
            {
                this.return_local_library();
            }

            /*
             *  Очистим временную папку
             */
            this.clear_temp_dir();
        }
    }
}