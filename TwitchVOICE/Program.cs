/*
 * Copyright (C) 2017-2018 Alimşah YILDIRIM <alimsahy@gmail.com>
 *
 * TwitchVOICE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TwitchVOICE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Speech.Recognition;

using WindowsMicrophoneMuteLibrary;

namespace TwitchVOICE
{
    class Program
    {
        static String[] badWords = { "bitch", "faggot", "dick", "asshole", "fuck", "nigga" };
        static SpeechRecognitionEngine engine;
        static Choices commands;
        static GrammarBuilder grammarBuilder;
        static Grammar grammer;
        static IRCClient irc;
        static WindowsMicMute mic;

        static Boolean slowMode;

        static void Main(string[] args)
        {
            slowMode = false;

            irc = new IRCClient("irc.twitch.tv", 6667, "twitch_username", "oauth:<your_token>");
            irc.joinRoom(irc.getUsername());

            Console.WriteLine("TwitchVOICE Chatbot [v1.0.0] ALPHA");
            Console.WriteLine("---------------");
            Console.WriteLine("Connected account: " + irc.getUsername());

            mic = new WindowsMicMute();
            engine = new SpeechRecognitionEngine();
            commands = new Choices();
            grammarBuilder = new GrammarBuilder();

            commands.Add(new String[] { "open sub mode", "close sub mode", "sikerim", "ananı", "slow mode" });
            grammarBuilder.Append(commands);
            grammer = new Grammar(grammarBuilder);

            engine.LoadGrammarAsync(grammer);
            engine.SetInputToDefaultAudioDevice();
            engine.RecognizeAsync(RecognizeMode.Multiple);
            engine.SpeechRecognized += engineRecognized;

            while (true)
            {
                string message = irc.readMessage();
                if (message.Contains("PRIVMSG"))
                {

                    Int32 intIndexParseSign = message.IndexOf('!');
                    String username = message.Substring(1, intIndexParseSign - 1);

                    intIndexParseSign = message.IndexOf(" :");
                    message = message.Substring(intIndexParseSign + 2);
                    Console.WriteLine(username + ": " + message);
                    if (username.Equals(irc.getUsername()))
                    {
                        if (message.Equals("!exit"))
                        {
                            irc.sendChatMessage("See you again guys! Have a nice day");
                            Environment.Exit(0);
                        }
                    }

                    if (message.Contains("have fun"))
                    {
                        irc.sendChatMessage("Have fun to you @" + username + "!");
                    }

                    if (message.Contains("low"))
                    {
                        irc.sendChatMessage("What did you say @" + username + " ?");
                    }

                    if (message.Equals("hi") || message.Equals("s.a"))
                    {
                        irc.sendChatMessage("Hi @" + username + ", welcome!");
                    }

                    if (message.Equals("!facebook"))
                    {
                        irc.sendChatMessage("https://facebook.com/<facebook_page>");
                    }

                    if (message.Equals("!twitter"))
                    {
                        irc.sendChatMessage("https://twitter.com/alimsahy");
                    }
                    foreach (String word in badWords)
                    {
                        if (message.Contains(word))
                        {
                            irc.sendChatMessage("/timeout " + username + " 1");
                            irc.sendChatMessage("Watch your words... @" + username);
                        }
                    }
                }
            }
        }

        static void engineRecognized(Object sender, SpeechRecognizedEventArgs args)
        {
            switch (args.Result.Text)
            {
                case "slow mode":
                {
                    if (!slowMode)
                    {
                        irc.sendChatMessage("/slow 60");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SLOW MODE ACTIVE");
                        Console.ForegroundColor = ConsoleColor.White;
                        slowMode = true;
                    }
                    else
                    {
                        irc.sendChatMessage("/slowoff");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("SLOW MODE PASSIVE");
                        Console.ForegroundColor = ConsoleColor.White;
                        slowMode = false;
                    }
                    break;
                }
                case "open sub mode":
                {
                    irc.sendChatMessage("/subscribers");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("SUB MODE ACTIVE");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
                case "close sub mode":
                {
                    irc.sendChatMessage("/subscribersoff");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("SUB MODE PASSIVE");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
                case "etc":
                {
                    break;
                }
            }
        }
    }
}
