using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SuperCarter.Model
{
    public class SingletonBase<T> where T : class
    {
        //public static Logger logger { get; set; }
        public readonly string AppPath = System.Windows.Forms.Application.StartupPath;

        public readonly string FOLDER_CONFIG = System.Windows.Forms.Application.StartupPath + @"\config\";
        public readonly string FOLDER_SCRIPT = System.Windows.Forms.Application.StartupPath + @"\script\";
        public readonly string FOLDER_Macro = System.Windows.Forms.Application.StartupPath + @"\script\macro\";
        public readonly string FOLDER_IMG = System.Windows.Forms.Application.StartupPath + @"\img\";
        public readonly string FOLDER_UTIL = System.Windows.Forms.Application.StartupPath + @"\util\";
        public readonly string FOLDER_ACCESS = System.Windows.Forms.Application.StartupPath + @"\access\";
        #region Members

        /// <summary>
        /// Static instance. Needs to use lambda expression
        /// to construct an instance (since constructor is private).
        /// </summary>
        private static readonly Lazy<T> sInstance = new Lazy<T>(() => CreateInstanceOfT());

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of this singleton.
        /// </summary>
        public static T Instance { get { return sInstance.Value; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of T via reflection since T's constructor is expected to be private.
        /// </summary>
        /// <returns></returns>
        private static T CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        #endregion
    }
}
