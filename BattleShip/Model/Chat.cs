using System;
using System.Collections.Generic;

namespace BattleShip.Model
{
    public sealed class ChatMessage
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid SenderId { get; }
        public string SenderName { get; }
        public string Text { get; }
        public DateTimeOffset SentAt { get; } = DateTimeOffset.UtcNow;

        public ChatMessage(User sender, string text)
        {
            SenderId = sender.Id;
            SenderName = sender.DisplayName;
            Text = text;
        }
    }

    public sealed class Chat
    {
        private readonly List<ChatMessage> _messages = new();
        public IReadOnlyList<ChatMessage> Messages => _messages;

        public ChatMessage Send(User sender, string text)
        {
            var message = new ChatMessage(sender, text);
            _messages.Add(message);
            return message;
        }
    }
}


