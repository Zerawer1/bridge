namespace BridgeNotifications;

// Служебная модель:
// хранит готовую комбинацию уведомления и данные для отправки.
internal sealed record NotificationScenario(
    Notification Notification,
    string Recipient,
    string Message);

// Интерфейс реализации:
// определяет общий контракт для всех каналов отправки.
public interface INotificationChannel
{
    string ChannelName { get; }

    void SendMessage(string subject, string message, string recipient);
}

// Конкретная реализация:
// отправка уведомления по электронной почте.
public sealed class EmailChannel : INotificationChannel
{
    public string ChannelName => "Email";

    public void SendMessage(string subject, string message, string recipient)
    {
        Console.WriteLine("[Email]");
        Console.WriteLine($"Кому: {recipient}");
        Console.WriteLine($"Тема: {subject}");
        Console.WriteLine($"Сообщение: {message}");
        Console.WriteLine("Статус: отправлено по электронной почте");
        Console.WriteLine();
    }
}

// Конкретная реализация:
// отправка уведомления через SMS.
public sealed class SmsChannel : INotificationChannel
{
    public string ChannelName => "SMS";

    public void SendMessage(string subject, string message, string recipient)
    {
        Console.WriteLine("[SMS]");
        Console.WriteLine($"Номер: {recipient}");
        Console.WriteLine($"Краткая тема: {subject}");
        Console.WriteLine($"Текст: {message}");
        Console.WriteLine("Статус: отправлено как SMS");
        Console.WriteLine();
    }
}

// Конкретная реализация:
// отправка Push-уведомления в приложение.
public sealed class PushChannel : INotificationChannel
{
    public string ChannelName => "Push";

    public void SendMessage(string subject, string message, string recipient)
    {
        Console.WriteLine("[Push]");
        Console.WriteLine($"Пользователь: {recipient}");
        Console.WriteLine($"Заголовок: {subject}");
        Console.WriteLine($"Текст: {message}");
        Console.WriteLine("Статус: отправлено как push-уведомление");
        Console.WriteLine();
    }
}

// Абстракция:
// базовый класс уведомления хранит ссылку на канал отправки.
public abstract class Notification
{
    private readonly INotificationChannel _channel;

    protected Notification(INotificationChannel channel)
    {
        _channel = channel;
    }

    // Название типа уведомления.
    public abstract string Name { get; }

    // Название используемого канала.
    public string ChannelName => _channel.ChannelName;

    // Общий алгоритм отправки:
    // конкретные уведомления формируют тему и текст,
    // а канал отвечает за способ доставки.
    public void Send(string recipient, string message)
    {
        string subject = BuildSubject();
        string body = BuildBody(message);

        _channel.SendMessage(subject, body, recipient);
    }

    protected abstract string BuildSubject();

    protected abstract string BuildBody(string message);
}

// Уточненная абстракция:
// обычное уведомление.
public sealed class BasicNotification : Notification
{
    public BasicNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override string Name => "Обычное уведомление";

    protected override string BuildSubject()
    {
        return "Обычное уведомление";
    }

    protected override string BuildBody(string message)
    {
        return $"Информация: {message}";
    }
}

// Уточненная абстракция:
// срочное уведомление.
public sealed class UrgentNotification : Notification
{
    public UrgentNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override string Name => "Срочное уведомление";

    protected override string BuildSubject()
    {
        return "Срочное уведомление";
    }

    protected override string BuildBody(string message)
    {
        return $"[Высокий приоритет] {message}";
    }
}

// Уточненная абстракция:
// уведомление с подтверждением.
public sealed class ConfirmationNotification : Notification
{
    public ConfirmationNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override string Name => "Уведомление с подтверждением";

    protected override string BuildSubject()
    {
        return "Уведомление с подтверждением";
    }

    protected override string BuildBody(string message)
    {
        return $"{message} Требуется подтверждение получения.";
    }
}

// Точка входа в программу.
internal static class Program
{
    private static void Main()
    {
        // Запускаем полную демонстрацию паттерна Bridge.
        ShowBridgeDemo();
    }

    // Демонстрация всех комбинаций:
    // 3 типа уведомлений x 3 канала отправки.
    private static void ShowBridgeDemo()
    {
        IReadOnlyList<NotificationScenario> scenarios = CreateScenarios();

        Console.WriteLine("СИСТЕМА УВЕДОМЛЕНИЙ: ПАТТЕРН BRIDGE");
        Console.WriteLine(new string('=', 45));
        Console.WriteLine();

        foreach (NotificationScenario scenario in scenarios)
        {
            Console.WriteLine($"Тип уведомления: {scenario.Notification.Name}");
            Console.WriteLine($"Канал отправки: {scenario.Notification.ChannelName}");
            Console.WriteLine(new string('-', 45));

            scenario.Notification.Send(scenario.Recipient, scenario.Message);
        }
    }

    // Формируем набор сценариев для показа того,
    // что любой тип уведомления можно связать с любым каналом.
    private static IReadOnlyList<NotificationScenario> CreateScenarios()
    {
        return
        [
            new(new BasicNotification(new EmailChannel()), "client@mail.com", "Ваш заказ принят в обработку."),
            new(new BasicNotification(new SmsChannel()), "+79990000001", "Ваша заявка зарегистрирована."),
            new(new BasicNotification(new PushChannel()), "user_01", "В приложении доступно новое сообщение."),

            new(new UrgentNotification(new EmailChannel()), "security@mail.com", "Обнаружена попытка входа в аккаунт."),
            new(new UrgentNotification(new SmsChannel()), "+79990000002", "Код подтверждения истекает через 1 минуту."),
            new(new UrgentNotification(new PushChannel()), "user_02", "Необходима срочная проверка профиля."),

            new(new ConfirmationNotification(new EmailChannel()), "manager@mail.com", "Подтвердите согласование документа."),
            new(new ConfirmationNotification(new SmsChannel()), "+79990000003", "Подтвердите запись на консультацию."),
            new(new ConfirmationNotification(new PushChannel()), "user_03", "Подтвердите получение уведомления.")
        ];
    }
}
