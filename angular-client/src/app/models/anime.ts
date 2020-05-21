import { Episode } from './episode';

export class Anime {
    id: number;
    show_id: number;
    season_id: number;
    show_type: number;
    number: number;
    episodes: Episode[];
    name: string;
    title: string;
    thumbnail: string;
    date_published: string;
    additional_info: string;
    director: string;

    constructor(data: Partial<Anime>) {
        Object.assign(this, data);
    }
}
