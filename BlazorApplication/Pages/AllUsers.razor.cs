using BlazorApplication.Features;
using BlazorApplication.Interfaces;
using BlazorApplication.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorApplication.Pages
{
    public partial class AllUsers
    {
        private IEnumerable<UserDto> userList;
        private IEnumerable<RoleDto> roleList;
        public bool ChangeMode { get; set; } = false;
        public string CurrentUserIdToChange { get; set; } = "-";
        public string NewRoleId { get; set; } = "";
        private UserParameters userParameters = new UserParameters();
        public MetaData MetaData { get; set; } = new MetaData();

        [Parameter]
        public bool successResponce { get; set; }

        [Inject]
        public IParticipantHttpRepository participantRepo { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IAccessTokenProvider accessTokenProvider { get; set; }

        [Inject]
        public IConfiguration configuration { get; set; }

        [Inject]
        public IUserHttpRepository UserRepo { get; set; }

        private Models.BackEndConnections _backEndConnections;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _backEndConnections = configuration.GetSection("ConnectionStrings").Get<BackEndConnections>();
            await GetUsers();
            roleList = await Http.GetFromJsonAsync<RoleDto[]>(_backEndConnections.CSharpUri + "user/allRoles");
        }

        private async System.Threading.Tasks.Task SelectedPage(int page)
        {
            userParameters.PageNumber = page;
            await GetUsers();
        }

        protected async System.Threading.Tasks.Task GetUsers()
        {
            var userPagingResponse = await UserRepo.GetUsersExtended(userParameters);
            userList = userPagingResponse.Items;
            MetaData = userPagingResponse.MetaData;
            successResponce = userPagingResponse.SuccessRequest;
        }

        public async void ChangeModeFun(string userIdToChange)
        {
            ChangeMode = !ChangeMode;
            CurrentUserIdToChange = userIdToChange;


            if (ChangeMode)
            {
                NewRoleId = userList.Where(u => u.Id == userIdToChange).Select(u => u.RoleId).FirstOrDefault();
            }
            else
            {
                var content = JsonSerializer.Serialize(new string[] { NewRoleId });
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var newRole = roleList.Where(r => r.Id == NewRoleId).FirstOrDefault();

                userList.Where(u => u.Id == userIdToChange).FirstOrDefault().RoleName = newRole.Name;
                userList.Where(u => u.Id == userIdToChange).FirstOrDefault().RoleId = newRole.Id;

                var putResult = await Http.PutAsync(_backEndConnections.CSharpUri + "user/assignRoles/" + userIdToChange, bodyContent);

                var putContent = await putResult.Content.ReadAsStringAsync();

                if (!putResult.IsSuccessStatusCode)
                {
                    throw new ApplicationException(putContent);
                }

                if (roleList.Where(r => r.Id == NewRoleId).FirstOrDefault().Name == "participant")
                {
                    var newUser = userList.Where(u => u.Id == userIdToChange).FirstOrDefault();

                    ParticipantParameters participantParameters = new ParticipantParameters
                    {
                        switchOff = true
                    };
                    var pagingResponse = await participantRepo.GetParticipantsByEmail(participantParameters, newUser.Email);
                    var currentUsers = pagingResponse.Items;

                    if (currentUsers.Count == 0)
                    {
                        var newParticipant = new Participant()
                        {
                            id = 0,
                            firstName = newUser.FirstName is not null ? newRole.Name : "unknown",
                            lastName = newUser.LastName is not null ? newRole.Name : "unknown",
                            email = newUser.Email,
                            userId = 1,
                            teamId = 0
                        };

                        await participantRepo.CreateParticipant(newParticipant);
                    }
                }
                else
                {
                    var newUser = userList.Where(u => u.Id == userIdToChange).FirstOrDefault();

                    ParticipantParameters participantParameters = new ParticipantParameters
                    {
                        switchOff = true
                    };
                    var pagingResponse = await participantRepo.GetParticipantsByEmail(participantParameters, newUser.Email);
                    var currentUsers = pagingResponse.Items;

                    if (currentUsers.Count > 0)
                    {
                        var toDeleteId = currentUsers.FirstOrDefault().id;
                        await participantRepo.DeleteParticipant(toDeleteId);
                    }
                }

                StateHasChanged();
            }
        }
    }
}
