using BertScout2025.Models;
using CommunityToolkit.Mvvm.Input;

namespace BertScout2025.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}