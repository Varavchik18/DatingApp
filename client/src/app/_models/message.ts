export interface Message {
    id: number;
    senderId: number;
    senderUserName: string;
    senderPhotoUrl: string;
    recepientId: number;
    recepientUserName: string;
    recepientPhotoUrl: string;
    content: string;
    dateRead?: Date;
    messageSent: Date;
}