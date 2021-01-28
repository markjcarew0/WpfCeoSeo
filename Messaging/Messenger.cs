// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Messenger.cs" company="Mark Carew">
//   Author: Mark Carew
// </copyright>
// <summary>
//   The messenger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Messaging
{
    using System;

    /// <summary>
    /// The messenger.
    /// </summary>
    public class Messenger
    {
        /// <summary>
        ///     Initializes static members of the <see cref="Messenger" /> class.
        /// </summary>
        static Messenger()
        {
            Singleton = new Messenger();
        }

        /// <summary>
        ///     the delegate signature for the message handling
        ///  N.B. the assignment of an empty delegate  = delegate { };
        /// This does not work in C# without this artifice
        /// </summary>
        public event EventHandler<MessageEventArgs> SendMessage = delegate { };

        /// <summary>
        ///     Gets the singleton.
        /// </summary>
        public static Messenger Singleton { get; }

        /// <summary>
        ///     Saves using Singleton Send Message Content everywhere
        ///     just simplifies the call to become intention revealing rather than
        ///     mechanism revealing
        /// </summary>
        /// <param name="message">the text of the message</param>
        /// <param name="messagePayload">an integer useful for enumeration or count usage</param>
        /// <param name="messageFor">a string that indicates the recipient</param>
        public static void SendMessageSingleton(string message, object messagePayload, string messageFor)
        {
            Singleton.SendMessageContent(message, messagePayload, messageFor);
        }

        /// <summary>
        ///     the instance method that sends the message payload for the singleton
        /// </summary>
        /// <param name="message">the text of the message</param>
        /// <param name="messagePayload">an integer useful for enumeration or count usage</param>
        /// <param name="messageFor">a string that indicates the recipient</param>
        private void SendMessageContent(string message, object messagePayload, string messageFor)
        {
            var sendIt = SendMessage;
            if (sendIt != null)
            {
                var details = new MessageEventArgs(message, messagePayload, messageFor);
                sendIt(this, details);
            }
        }
    }
}
