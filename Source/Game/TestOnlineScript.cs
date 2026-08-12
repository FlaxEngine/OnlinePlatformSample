using System;
using System.Collections.Generic;
using System.Text;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Online;

namespace Game;

/// <summary>
/// TestOnlineScript Script.
/// </summary>
public class TestOnlineScript : Script
{
    private StringBuilder _sb = new StringBuilder();
    private List<Texture> _avatars = new List<Texture>();
    private float _lastUpdate;
    private IOnlinePlatform _online;

    public float RefreshTime = 1.0f;
    public ControlReference<ContainerControl> FriendsPanel;
    public ControlReference<Image> AvatarImage;
    public ControlReference<Label> InfoLabel;

    /// <inheritdoc/>
    public override void OnStart()
    {
        // Ininitialize online platform (see Game.Build.cs which references them)
#if PLATFORM_WINDOWS || PLATFORM_LINUX || PLATFORM_MAC
        _online = new OnlinePlatformSteam();
#elif PLATFORM_XBOX_SCARLETT || PLATFORM_XBOX_ONE
        _online = new OnlinePlatformXboxLive();
#elif PLATFORM_SWITCH
        _online = new OnlinePlatformSwitch();
#elif PLATFORM_PS4
        _online = new OnlinePlatformPS4();
#elif PLATFORM_PS5
        _online = new OnlinePlatformPS5();
#endif
        if (_online == null)
            return;
        Online.Initialize(_online);

        // Run the first update
        UpdateText();
    }

    /// <inheritdoc/>
    public override void OnUpdate()
    {
        // Update once a while to test the integration
        if (Time.GameTime - _lastUpdate > RefreshTime)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        // Cleanup before fetching a new data
        foreach (var avatar in _avatars)
            Destroy(avatar);
        _avatars.Clear();
        _sb.Clear();
        var vScroll = (FriendsPanel.Control.Parent as Panel)?.VScrollBar;
        var scroll = vScroll?.Value ?? 0;
        FriendsPanel.Control.Parent.LockChildrenRecursive();
        FriendsPanel.Control.DisposeChildren();
        AvatarImage.Control.Brush = null;

        UodateTextInner();

        InfoLabel.Control.Text = _sb.ToString();
        if (vScroll != null)
            vScroll.Value = scroll;
        FriendsPanel.Control.Parent.UnlockChildrenRecursive();
        FriendsPanel.Control.Parent.PerformLayout();
        _lastUpdate = Time.GameTime;
    }

    private void UodateTextInner()
    {
        // Platform info
        if (_online == null)
        {
            _sb.Append("Online: null");
            return;
        }
        _sb.AppendLine($"Online: {_online.GetType().Name}");

        // User info
        if (_online.GetUser(out var user))
        {
            if (_online.UserLogin())
            {
                _sb.AppendLine("Failed to login");
                return;
            }
            if (_online.GetUser(out user))
            {
                _sb.AppendLine("Failed to get user info");
                return;
            }
        }
        _sb.AppendLine($"User: {user.Name}, state: {user.PresenceState}");
        if (!_online.GetUserAvatar(user, out var avatar))
        {
            _avatars.Add(avatar);
            AvatarImage.Control.Brush = new TextureBrush(avatar);
        }

        // Achievements
        if (!_online.GetAchievements(out var achievements))
        {
            _sb.AppendLine("Achievements: " + achievements.Length);
            foreach (var a in achievements)
            {
                _sb.AppendLine($"   > {a.Name} ({a.Title}): progress {a.Progress}{(a.IsHidden ? " (hidden)" : "")}");
            }
        }

        // User friends info
        if (_online.GetFriends(out var friends))
        {
            _sb.AppendLine("Failed to get user friends");
        }
        else
        {
            _sb.AppendLine("Friends: " + friends.Length);
            foreach (var friend in friends)
            {
                var friendItem = new Label
                {
                    Height = 32,
                    Text = $"Friend: {friend.Name}, state: {friend.PresenceState}",
                };
                if (!string.IsNullOrEmpty(friend.PresenceStatus))
                    friendItem.Text += $", {friend.PresenceStatus}";
                if (!_online.GetUserAvatar(friend, out avatar))
                {
                    _avatars.Add(avatar);
                    var friendImage = new Image
                    {
                        Size = new Float2(friendItem.Height),
                        Brush = new TextureBrush(avatar),
                        Parent = friendItem,
                    };
                }
                FriendsPanel.Control.AddChild(friendItem);
            }
        }
    }
}
