namespace DS_OS.Engine.CommandHandler.BaseClass;

public enum CommandType
{
    None = 0,
    PROCESSCREATE,
    PROCESSDELETE,
    FILECREATE,
    FILEDELETE,
    DIRECTORYCREATE,
    SHUTDOWN
}