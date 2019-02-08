namespace Api.Domain.Enums
{
    public enum Playback
    {
        // Playback not yet been invoked in client
        NotActive = 0,

        // Playback invoked and currently playing
        Playing = 1,

        // Playback invoked but currently paused
        Paused = 2
    }
}