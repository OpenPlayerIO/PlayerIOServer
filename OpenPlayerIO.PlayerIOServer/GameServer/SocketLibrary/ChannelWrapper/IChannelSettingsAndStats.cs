namespace SocketSlim.ChannelWrapper
{
    public interface IChannelSettingsAndStats
    {
        /// <summary>
        /// Gets or sets the behavior for the channel when the size of outgoing queue in items
        /// exceeds <see cref="MaxOutgoingQueueLength"/>.
        /// </summary>
        OutgoingQueueOverflowBehavior OutgoingQueueOverflowBehavior { get; set; }

        /// <summary>
        /// Gets or sets a length of ougoing queue to trigger <see
        /// cref="OutgoingQueueOverflowBehavior"/>. Must be greater than zero to take effect.
        ///
        /// For the optimal performance, this should be set to <see cref="int.MaxValue"/> to save two
        /// comparison operations, if you want to turn off the check;
        /// </summary>
        int MaxOutgoingQueueLength { get; set; }

        /// <summary> Gets the immediate send queue item count. </summary>
        int CurrentOutgoingQueueLength { get; }

        /// <summary> Gets the immediate send queue cumulative size in bytes. </summary>
        long CurrentOutgoingQueueSize { get; }

        /// <summary>
        /// Gets the maximum send queue length in items throughout the life of a channel wrapper.
        /// </summary>
        int PeakOutgoingQueueLength { get; }

        /// <summary> Gets the number of bytes that were sent through the socket. </summary>
        long SentByteCount { get; }

        /// <summary> Gets the number of bytes that were sent through the socket. </summary>
        long ReceivedByteCount { get; }
    }
}