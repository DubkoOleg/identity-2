namespace OlMag.Manufacture2.Data
{
    public enum EnRole
    {
        /// <summary>
        /// Админ
        /// </summary>
        Admin = 1,
        /// <summary>
        /// Aдминистратор пользователей
        /// </summary>
        UserAdministrator = 2,
        /// <summary>
        /// Менеджер по продажам
        /// </summary>
        SalesManager = 3,
        /// <summary>
        /// Менеджер производства
        /// </summary>
        ProductionManager = 4,
        /// <summary>
        /// Главный конструктор
        /// </summary>
        ChiefDesigner = 5,
        /// <summary>
        /// Конструктор
        /// </summary>
        Designer = 6,
        /// <summary>
        /// Технолог
        /// </summary>
        Technologist = 7,
        /// <summary>
        /// Распечатыватель
        /// </summary>
        Printer = 8,
        /// <summary>
        /// Мастер ЧПУ
        /// </summary>
        MasterCnc = 9,
        /// <summary>
        /// Мастер по механической обработке
        /// </summary>
        MasterOfMechanicalProcessing = 10,
        /// <summary>
        /// Мастер по лазерной резке
        /// </summary>
        MasterOfLaser = 11,
        /// <summary>
        /// Бригада ЧПУ
        /// </summary>
        TeamCnc = 12,
        /// <summary>
        /// Бригада по механической обработке
        /// </summary>
        TeamOfMechanicalProcessing = 13,
        /// <summary>
        /// Бригада по лазерной резке
        /// </summary>
        TeamOfLaser = 14,
        /// <summary>
        /// Снабженец
        /// </summary>
        Supplier = 15,
        /// <summary>
        /// Бригада сборки
        /// </summary>
        TeamAssembly = 16,
        /// <summary>
        /// ОТК (Отдел технического контроля)
        /// </summary>
        ControlDepartment = 17,
        /// <summary>
        /// Упаковщик
        /// </summary>
        Packer = 18,
    }
}
