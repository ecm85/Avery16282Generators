import React, { Component } from 'react';

export class Brewcrafters extends Component {
    displayName = Brewcrafters.name

    constructor(props) {
        super(props);
        this.state = {
            generating: false,
            error: null,
            downloadLink: null,
            labelsToSkip: 0
        };
    }

    handleGenerateClick = () => {
        const { labelsToSkip } = this.state;
        this.setState({ generating: true, error: null, downloadLink: null });
        const body = JSON.stringify({
            labelsToSkip
        });
        fetch('api/PdfGenerator/GenerateBrewcrafters', {
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
            generating,
            downloadLink,
            error,
            labelsToSkip
        } = this.state;
        return (
            <div>
                <h1>Brewcrafters</h1>
                <div>
                    <h3>Label Spots to Skip</h3>
                    <input type={'number'} onChange={(event) => this.setState({ labelsToSkip: +event.target.value })} value={labelsToSkip} />
                </div>
                <div>
                    {!generating && <button type='button' className='btn btn-primary' onClick={this.handleGenerateClick}>Generate Labels</button>}
                    {generating && <button type='button' className='btn btn-primary disabled'>Generating...</button>}
                    {downloadLink != null && <h3>Generated File: <a target="_blank" href={downloadLink}>Link</a> (Link valid for 1 day)</h3>}
                    {error != null && <div>Error generating file or uploading to S3: {error}</div>}
                </div>
            </div>
        );
    }
}
