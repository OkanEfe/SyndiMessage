using SyndiMessage.Attributes;
using SyndiMessage.Test.Unit.Attributes.MessageInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SyndiMessage.Test.Unit.Attributes;
public class BindingDeclarationAttributeTest
{
    [Fact]
    public void MessageType_ShouldHave_BindingDeclaration()
    {
        SyndiTestMessage message = new SyndiTestMessage();

        var attribute = message.GetType().GetCustomAttributes(typeof(BindingDeclarationAttribute), false).Any();

        Assert.True(attribute);
    }
}
