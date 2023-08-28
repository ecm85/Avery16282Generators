import React, { Component } from 'react';

export class ArkhamHorrorLCG extends Component {
    displayName = ArkhamHorrorLCG.name

    constructor(props) {
        super(props);
        this.state = {
            initializing: true,
            generating: false,
            error: null,
            downloadLink: null,
            allCycles: [],
            selectedCycles: ['Base']
        };
        fetch('api/PdfGenerator/GetArkhamHorrorLcgCycles')
            .then(response => response.json())
            .then(data => {
                this.setState({ allCycles: data, initializing: false });
            });
    }

    handleCyclesChange = (event) => {
        const selectedCycles = Array.from(event.target.selectedOptions).map(option => option.value);
        this.setState({ selectedCycles });
    }

    handleGenerateClick = () => {
        const { selectedCycles } = this.state;
        this.setState({ generating: true, error: null, downloadLink: null });
        const body = JSON.stringify({
            selectedCycles
        });
        fetch('api/PdfGenerator/GenerateArkhamHorrorLcg', {
            method: 'POST',
            body,
            headers: { 'Content-Type': 'application/json', },
        }).then(response => {
            if (response.ok) {
                response.text().then(data => this.setState({ generating: false, downloadLink: data }));
            }
            else
                this.setState({ generating: false, error: response.statusText });
        }, error => {
            this.setState({ generating: false, error });
        });
    }

    render() {
        const {
            initializing,
            generating,
            allCycles,
            selectedCycles,
            downloadLink,
            error
        } = this.state;
        return (
            <div>
                <h1>Arkham Horror LCG</h1>
                {initializing && <p><em>Initializing...</em></p>}
                {!initializing &&
                    <div>
                        <select
                            className='selectmultiple form-control'
                            multiple={true}
                            size={allCycles.length}
                            disabled={generating}
                            onChange={this.handleCyclesChange}
                            value={selectedCycles}>
                            {allCycles.map((cycle => <option key={cycle}>{cycle}</option>))}
                        </select>
                        {!generating && <button type='button' className='btn btn-primary' onClick={this.handleGenerateClick}>Generate Labels</button>}
                        {generating && <button type='button' className='btn btn-primary disabled'>Generating...</button>}
                        {downloadLink != null && <h3>Generated File: <a target="_blank" href={downloadLink}>Link</a> (Link valid for 1 day)</h3>}
                        {error != null && <div>Error generating file or uploading to S3: {error}</div>}
                    </div>
                }
            </div>
        );
    }
}
