using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Helpers;

namespace Api.Business.Helpers
{
    public class StatusUpdateHelper : IStatusUpdateHelper
    {/*
         * Generates a PlaybackNotification (PlaybackStatusUpdate) for the given party
         */
        public PlaybackStatusUpdate GeneratePlaybackStatusUpdate(Party party, bool includeAdminFields = false)
        {
            var update = new PlaybackStatusUpdate()
            {
                Uri = party.CurrentTrack.Uri,
                Artist = party.CurrentTrack.Artist,
                DurationMillis = party.CurrentTrack.DurationMillis,
                Title = party.CurrentTrack.Title
            };

            if (includeAdminFields)
            {
                update.DeviceId = party.Owner.CurrentDevice?.DeviceId;
                update.DeviceName = party.Owner.CurrentDevice?.Name;
                update.PlaybackState = party.Playback;
            }

            return update;
        }

    }
}