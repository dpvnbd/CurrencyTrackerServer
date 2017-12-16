import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { UpdateSource } from '../../shared';

export interface ChangePercentage {
    currency: string;
    percentChanged: number;
}

@Injectable()
export class ChangesStatsService {


    constructor(private httpClient: HttpClient) { }


    public getPercentages(source: UpdateSource): any {
        return this.httpClient.get('/api/changes/stats/' + source)
            .map(data => data as ChangePercentage[]).toPromise();
    }

    public convertPercentagesToChartData(changes: ChangePercentage[]): any {
        const data: any = {
            chartType: 'Histogram',
            dataTable: [
                ['Валюта', 'Изменение, %']
            ]
        };

        for (const c of changes) {
            data.dataTable.push([c.currency, c.percentChanged]);
        }

        return data;
    }

}
