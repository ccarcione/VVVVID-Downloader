export class Episode {
    id: number;
    season_id: number;
    video_id: number;
    number: string;
    title: string;
    thumbnail: string;
    views: number;
    length: number;
    description: string;
    expired: boolean;
    seen: boolean;
    playable: boolean;
    ondemand_type: number;
    vod_mode: number;
    videoLink: string;
    embed_info: string;
    embed_info_sd: string;
    video_shares: number;
    video_likes: number;

    constructor(data: Partial<Episode>) {
        Object.assign(this, data);
    }
}
