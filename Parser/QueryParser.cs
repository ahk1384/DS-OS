using DS_OS.Engine.CommandHandler.BaseClass;
using DS_OS.Exceptions;

namespace DS_OS.Parser;

public class QueryParser : IQueryParser
{
    public Command Parse(String query)
    {
        if (query == "")
        {
            throw new InvalidQueryException();
        }
        string type = query.Substring(0, query.IndexOf('('));
        string[] args = query.Substring(query.IndexOf('(')+1, query.IndexOf(')')-query.IndexOf('(')-1).Split(',');
        if (CommandType.TryParse(type.ToUpper(), out CommandType commandType))
        {
            Command command = new Command(commandType, new Dictionary<string, string>());
            foreach (string value in args)
            {
                string[] parms = value.Split(":");
                if (parms.Length == 2)
                {
                    if (ParametrsType.TryParse(parms[0].ToUpper(), out ParametrsType _))
                    {
                        command.AddParams(parms[0],parms[1]);
                    }
                    else
                    {
                        throw new InvalidParametersException();
                    }
                }
                else
                {
                    throw new InvalidParametersException();
                }

            }
            return command;
        }
        else
        {
            throw new InvalidQueryException();
        }
    }

}