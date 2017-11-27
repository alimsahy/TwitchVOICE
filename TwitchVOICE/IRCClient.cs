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
using System.Text;

namespace TwitchVOICE
{
    class IRCClient
    {
        private String userName;
        private String channel;

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IRCClient(String ip, Int32 port, String userName, String password)
        {
            this.userName = userName;

            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + userName);
            outputStream.WriteLine("USER " + userName + " 8 * :" + userName);
            outputStream.Flush();
        }

        public void joinRoom(String channel)
        {
            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
            this.channel = channel;
        }

        public void sendIrcMessage(String message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void sendChatMessage(String message)
        {
            sendIrcMessage(":" + getUsername() + "!" + getUsername() + "@" + getUsername() + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
        }

        public String readMessage()
        {
            String message = inputStream.ReadLine();
            return message;
        }

        public String getUsername()
        {
            return userName;
        }
    }
}