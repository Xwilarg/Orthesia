using System;

namespace Orthesia
{
    public static class Sentences
    {
        public readonly static string hiStr = "Hi";
        public readonly static string deleteConfirm = "Are you sure you want to close your ticket ?";
        public readonly static string chanAlreadyExist = "You already have a opened support channel.";
        public static string chanCreated(string chanName) { return ("The channel " + chanName + " was created for you."); }
        public readonly static string wrongChan = "You must be in your support channel to close it.";
        public readonly static string needWait = "You need to wait at least 10 minutes before opening a new ticket.";
        public readonly static string help = "**Ticket**: Open a ticket in a private channel" + Environment.NewLine +
                                            "**Close**: Close the currently opened ticket";
    }
}