namespace OnlineOrderCart.Web.Helpers
{
    public static class UserRoles
    {
        public const string PowerfullUser = "PowerfulUser";
        public const string Kam = "Kam";
        public const string KAMAdministrador = "KAM-Administrador";
        public const string Coordinador = "Coordinador";
        public const string CoordinadorAdministrador = "Coordinador-Administrador";
        public const string Distributor = "Distributor";
        public const string Adminstrator = PowerfullUser + KAMAdministrador + CoordinadorAdministrador;
        public const string KamCoord = Kam + Coordinador;
    }
}
