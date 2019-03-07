using System;


namespace AdaptivBot
{
    public class GlobalConfigValues
    {
        private static GlobalConfigValues instance = null;

        private static readonly object padlock = new object();


        
        private GlobalConfigValues()
        { }

        public static GlobalConfigValues Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalConfigValues();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
