using System.IO;

namespace RaspMat.Interfaces
{
    internal interface IFileService
    {

        Stream OpenFile();
        Stream NewFile();

    }
}
