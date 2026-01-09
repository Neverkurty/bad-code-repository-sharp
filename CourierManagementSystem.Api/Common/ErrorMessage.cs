public static class ErrorMessages
{
    public static string UserWithLoginExists(string login) =>
        $"Пользователь с логином '{login}' уже существует";
}
