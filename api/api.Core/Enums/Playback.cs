namespace api.Domain.Enums
{
    public enum Playback
    {
        // Playback not yet been invoked in client
        NOT_ACTIVE = 0,

        // Playback invoked and currently playing
        PLAYING = 1,

        // Playback invoked but currently paused
        PAUSED = 2
    }
}