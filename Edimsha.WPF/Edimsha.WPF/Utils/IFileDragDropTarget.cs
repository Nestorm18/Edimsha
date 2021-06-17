namespace Edimsha.WPF.Utils
{
    /// <summary>
    /// IFileDragDropTarget Interface
    /// </summary>
    public interface IFileDragDropTarget
    {
        void OnFileDrop(string[] filepaths);
    }
}