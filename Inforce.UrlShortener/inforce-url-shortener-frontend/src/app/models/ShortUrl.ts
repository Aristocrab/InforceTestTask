import { User } from "./User";

export interface ShortUrl {
    id: string;
    originalUrl: string;
    shortCode: string;
    createdDate: Date;
    createdBy: User;
}