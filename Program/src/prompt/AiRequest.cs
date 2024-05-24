using System.Text;
using System.Text.Json;
namespace GlobalNameSpace;

public class AiRequest(IAiModel model)
{
    public IAiModel Model{ get; set; } = model;
    public List<ChatMessage> Messages { get; set; } = [];
    public bool Stream { get; set; } = false;
    public int MaxOutputTokens { get; set; } = model.MaxOutputTokens;
    public double Temperature { get; set; } = 0.8;

    public void LoadTwoWayConversation(List<string> messages)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (i % 2 == 0)
            {
                AddUserMessage(messages[i]);
            }
            else
            {
                AddAssistantMessage(messages[i]);
            }
        }
    }

    public void AddUserMessage(string content)
    {
        string userRole = Model.ApiAccess.ChatRoles.UserRole;
        Messages.Add(new ChatMessage(userRole, content));
    }

    public void AddAssistantMessage(string content)
    {
        string assistantRole = Model.ApiAccess.ChatRoles.AssistantRole;
        Messages.Add(new ChatMessage(assistantRole, content));
    }

    public List<object> MessagesToJsonObject()
    {
        var jsonMessages = new List<object>();
        foreach (ChatMessage message in Messages)
        {
            jsonMessages.Add(message.ToJsonObject());
        }
        return jsonMessages;
    }
}