using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using System;

namespace Sks365.Ippica.Application.Utility.EmailSender
{
    public static class EmailTemplate
    {
        public static string GetUndoCancelEmailText(Bet bet)
        {
            return "<table style='width: 100%;height: 35px;color: rgb(255, 0, 0); border-collapse: collapse; border: 1px solid rgb(6, 6, 6); background-color: rgb(255, 0, 0);'><tbody><tr>" +
                   "<td style = 'width: 100%; border: medium solid rgb(255, 255, 255);'><span style = 'font-family: Calibri, sans-serif;'><strong><span style = 'color: rgb(239, 239, 239);'> " +
                   " IPPICA - Important notification " +
                   "</span></strong></span><span style = 'color: rgb(239, 239, 239);'></span><br></td></tr></tbody></table><p><span style='font-family: Calibri, sans-serif;'>" +

                   "Action 'Cancel Refund' cannot be completed due to insufficient funds on the user's wallet.</span><br><span style = 'font-family: Calibri, sans-serif;'><br>" +

                   "<strong> Ticket ID: " + bet.ExternalId + "</strong><br>" +
                   " Amount: " + Math.Round(bet.Stake ?? 0, 2) + " " + bet.CurrencyId.Value.GetDescription() + "<br>" +
                   " User ID: " + bet.UserId + "<br><br> " +
                   "<strong>Solution 1:</strong> Wait for the client to make a valid deposit via Cashier.<br>" +
                   "<strong> Solution 2 </strong> (recommended): </span><span style= 'font-family: Calibri, sans-serif;'> Contact administrator to manually set the proper state for the selected ticket and " +
                   "stop infinitive, failed HTTP attempts created by IPPICA provider.</span><span style = 'font-family: Calibri, sans-serif;'><br><br>" +
                   "Instructions for developers (to stop infinitive, failed calls to the API follow these steps): <br>" +
                   "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- create fake BetRequest (type = 4)<br>" +
                   "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- create fake BetTransaction (type: stake) and link with fake BetRequest (copy the values from the last valid stake)<br>" +
                   "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- set the bet status to PLACED (if it's not already)<br>" +
                   "<br><br>" +
                   "<i>The mail is auto generated, please do not reply.</i><br><br><br><br><br></span></p>";
        }
    }
}
