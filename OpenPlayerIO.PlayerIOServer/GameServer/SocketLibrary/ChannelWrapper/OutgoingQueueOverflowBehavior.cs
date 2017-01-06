namespace SocketSlim.ChannelWrapper
{
    /// <summary> Enum which describes available outgoing queue overflow behaviors. </summary>
    public enum OutgoingQueueOverflowBehavior
    {
        /// <summary> Ignore the queue going over the queue length. </summary>
        Ignore,

        /// <summary> Discard sent message silently when the queue is full. </summary>
        DiscardSilently,

        /// <summary>
        /// Discard sent message and throw exception in Send method to notify the caller.
        /// </summary>
        DiscardException,

        /// <summary>
        /// Remove some of the messages at the start of a queue to make room for the message being sent.
        /// </summary>
        RetireOldMessages,

        /// <summary> Clear the queue entirely when it goes over the maximum item count. </summary>
        ClearQueue,

        /// <summary> Close channel when the queue goes over the maximum item count. </summary>
        Close
    }
}