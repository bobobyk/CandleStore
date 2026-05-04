using System.Windows;
using CandleStore.Data;

namespace CandleStore
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Инициализация базы данных тестовыми данными
            using (var db = new AppDbContext())
            {
                DbInitializer.Initialize(db);
            }
        }
    }
}
