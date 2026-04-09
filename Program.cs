// Демонстрация паттерна Bridge:
// есть разные типы уведомлений и разные способы их отправки.

// Здесь создаем разные комбинации:
// любой тип уведомления можно отправить через любой канал.
Notification basicByEmail = new BasicNotification(new EmailChannel());
Notification urgentBySms = new UrgentNotification(new SmsChannel());
Notification confirmationByPush = new ConfirmationNotification(new PushChannel());
Notification urgentByEmail = new UrgentNotification(new EmailChannel());

// Вызываем отправку уведомлений.
basicByEmail.Send("client@mail.com", "Ваш заказ оформлен.");
urgentBySms.Send("+79991234567", "Обнаружена попытка входа в аккаунт.");
confirmationByPush.Send("user_42", "Подтвердите запись на консультацию.");
urgentByEmail.Send("admin@mail.com", "Сервер временно недоступен.");

// Интерфейс реализации:
// задает общий контракт для всех каналов отправки.
public interface INotificationChannel
{
    void SendMessage(string subject, string message, string recipient);
}

// Конкретная реализация:
// отправка уведомления по электронной почте.
public sealed class EmailChannel : INotificationChannel
{
    public void SendMessage(string subject, string message, string recipient)
    {
        Console.WriteLine("[Email]");
        Console.WriteLine($"Кому: {recipient}");
        Console.WriteLine($"Тема: {subject}");
        Console.WriteLine($"Сообщение: {message}");
        Console.WriteLine();
    }
}

// Конкретная реализация:
// отправка уведомления через SMS.
public sealed class SmsChannel : INotificationChannel
{
    public void SendMessage(string subject, string message, string recipient)
    {
        Console.WriteLine("[SMS]");
        Console.WriteLine($"Кому: {recipient}");
        Console.WriteLine($"Сообщение: {subject} | {message}");
        Console.WriteLine();
    }
}

// Конкретная реализация:
// отправка Push-уведомления в приложение.
public sealed class PushChannel : INotificationChannel
{
    public void SendMessage(string subject, string message, string recipient)
    {
        Console.WriteLine("[Push]");
        Console.WriteLine($"Получатель: {recipient}");
        Console.WriteLine($"Заголовок: {subject}");
        Console.WriteLine($"Текст: {message}");
        Console.WriteLine();
    }
}

// Абстракция:
// базовый класс уведомления хранит ссылку на канал отправки.
public abstract class Notification
{
    protected readonly INotificationChannel Channel;

    protected Notification(INotificationChannel channel)
    {
        Channel = channel;
    }

    public abstract void Send(string recipient, string message);
}

// Уточненная абстракция:
// обычное уведомление.
public sealed class BasicNotification : Notification
{
    public BasicNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override void Send(string recipient, string message)
    {
        Channel.SendMessage("Обычное уведомление", message, recipient);
    }
}

// Уточненная абстракция:
// срочное уведомление.
public sealed class UrgentNotification : Notification
{
    public UrgentNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override void Send(string recipient, string message)
    {
        Channel.SendMessage("Срочное уведомление", $"[Срочно] {message}", recipient);
    }
}

// Уточненная абстракция:
// уведомление с подтверждением.
public sealed class ConfirmationNotification : Notification
{
    public ConfirmationNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override void Send(string recipient, string message)
    {
        Channel.SendMessage(
            "Уведомление с подтверждением",
            $"{message} Подтвердите получение ответа.",
            recipient);
    }
}
