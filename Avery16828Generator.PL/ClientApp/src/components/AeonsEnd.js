import React, { Component } from 'react';

export class AeonsEnd extends Component {
    displayName = AeonsEnd.name

    constructor(props) {
        super(props);
        this.state = {
            initializing: true,
            generating: false,
            error: null,
            downloadLink: null,
            allExpansions: [],
            selectedExpansions: ['Base']
        };
        fetch('api/PdfGenerator/GetAeonsEndExpansions')
            .then(response => response.json())
            .then(data => {
                this.setState({ allExpansions: data, initializing: false });
            });
    }

    handleExpansionsChange = (event) => {
        const selectedExpansions = Array.from(event.target.selectedOptions).map(option => option.value);
        this.setState({ selectedExpansions });
    }

    handleGenerateClick = () => {
        const { selectedExpansions } = this.state;
        this.setState({ generating: true, error: null, downloadLink: null });
        const body = JSON.stringify({
            selectedExpansionNames: selectedExpansions
        });
        fetch('api/PdfGenerator/GenerateAeonsEnd', {
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
            allExpansions,
            selectedExpansions,
            downloadLink,
            error
        } = this.state;
        return (
            <div>
                <h1>Aeon's End</h1>
                {initializing && <p><em>Initializing...</em></p>}
                {!initializing &&
                    <div>
                        <select
                            className='selectmultiple form-control'
                            multiple={true}
                            size={allExpansions.length}
                            disabled={generating}
                            onChange={this.handleExpansionsChange}
                            value={selectedExpansions}>
                            {allExpansions.map((expansion => <option key={expansion}>{expansion}</option>))}
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
