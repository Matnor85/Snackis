namespace Snackis.Services;

// AI-assisterad olästa meddelande räknare:
// Scoped service som delas av alla komponenter för samma besökare.
// Conversation säger till via NotifyChanged, och NavMenu lyssnar på Changed
// och räknar om antalet olästa.
public class UnreadMessagesState
{
    public event Action? Changed;

    public void NotifyChanged() => Changed?.Invoke();
}