// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEventArgs.cs" company="Mark Carew">
//   Author: Mark Carew
// </copyright>
// <summary>
//   The message event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Messaging
{
    using System;

    /// <summary>
    ///     The message event args.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageEventArgs" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="content">
        ///     The count.
        /// </param>
        /// <param name="messageFor">
        ///     The message for.
        /// </param>
        public MessageEventArgs(string message, object content, string messageFor)
        {
            Message = message;
            Content = content;
            MessageFor = messageFor;
        }

        /// <summary>
        ///     Gets or sets the count.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the message for.
        /// </summary>
        public string MessageFor { get; set; }
    }
}
