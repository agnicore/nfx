using System;
using System.Linq;
using NFX.Scripting;

using NFX.ApplicationModel;
using NFX.Web.Messaging;

namespace NFX.ITest.Web.Messaging
{
  [Runnable]
  public class TwilioSinkTest : IRunnableHook
  {
    public const string CONFIG =
    @"nfx
      {
        messaging
        {
          sink
          {
            type='NFX.Web.Messaging.TwilioSink, NFX.Web'
            name='Twilio'
            account-sid='AC31483195bf01c8be5c0a6883d1288055'  // $(~TWILIO_ACCOUNT_SID)
            auth-token='dfbedd7b09865fafaebd1605728f6e2c'     // $(~TWILIO_AUTH_TOKEN)
            from='+15005550006'
          }
        }
      }";

    private ServiceBaseApplication m_App;
    private TwilioSink m_Sink;

    void IRunnableHook.Prologue(Runner runner, FID id)
    {
      var config = CONFIG.AsLaconicConfig(handling: ConvertErrorHandling.Throw);
      m_App = new ServiceBaseApplication(null, config);

      m_Sink = (TwilioSink)((MessageService)MessageService.Instance).Sink;
      Aver.IsNotNull(m_Sink);

      Aver.IsTrue(m_Sink.Name.EqualsOrdIgnoreCase("Twilio"));
      Aver.IsTrue(m_Sink.SupportedChannels == MsgChannels.SMS);
      Aver.IsTrue(m_Sink.SupportedChannelNames.Contains("Twilio"));

      Aver.IsTrue(m_Sink.Running);
    }

    bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
    {
      DisposableObject.DisposeAndNull(ref m_App);
      return false;
    }

    [Run]
    public void SendSMS()
    {
      var sms = new Message(null)
      {
        AddressTo = "as { a { nm=Nick cn=Twilio ca=+15005550005 } }",
        Body = "Test SMS. Тестирование SMS"
      };
      var sent = m_Sink.SendMsg(sms);
      Aver.IsTrue(sent);
    }

    [Run]
    public void BadRequest_Throw()
    {
      try
      {
        var sms = new Message(null)
        {
          AddressTo = "as { a { nm=Nick cn=Twilio ca=+15005550009 } }",
          Body = "Test SMS"
        };
        var sent = m_Sink.SendMsg(sms);
      }
      catch (Exception error)
      {
        Aver.IsTrue(error.Message.EqualsOrdIgnoreCase("Sending message on sink 'Twilio' has not succeeded"));
      }
    }

    [Run]
    public void BadRequest_Ignore()
    {
      var oldMode = m_Sink.ErrorHandlingMode;
      m_Sink.ErrorHandlingMode = SendMessageErrorHandling.Ignore;

      var sms = new Message(null)
      {
        AddressTo = "as { a { nm=Nick cn=Twilio ca=+15005550009 } }",
        Body = "Test SMS"
      };
      var sent = m_Sink.SendMsg(sms);
      Aver.IsFalse(sent);
      m_Sink.ErrorHandlingMode = oldMode;
    }
  }
}
