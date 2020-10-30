import React, { Component } from 'react';

export class Legendary extends Component {
    displayName = Legendary.name

    constructor(props) {
        super(props);
        this.state = {
            initializing: true,
            generating: false,
            error: null,
            downloadLink: null,
            allExpansions: [],
            selectedExpansions: ['Legendary'],
            includeSpecialSetupCards: false
        };
        fetch('api/PdfGenerator/GetLegendaryExpansions')
            .then(response => response.json())
            .then(data => {
                this.setState({ allExpansions: data, initializing: false });
            });
    }

    handleExpansionsChange = (event) => {
        const selectedExpansions = Array.from(event.target.selectedOptions).map(option => option.value);
        this.setState({ selectedExpansions });
    }

    handleIncludeSpecialSetupCardsChange = () => {
        const { includeSpecialSetupCards } = this.state;
        this.setState({ includeSpecialSetupCards: !includeSpecialSetupCards });
    }

    handleGenerateClick = () => {
        const { selectedExpansions, includeSpecialSetupCards } = this.state;
        this.setState({ generating: true, error: null, downloadLink: null });
        const body = JSON.stringify({
            selectedExpansionNames: selectedExpansions,
            includeSpecialSetupCards
        });
        fetch('api/PdfGenerator/GenerateLegendary', {
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
            includeSpecialSetupCards,
            downloadLink,
            error
        } = this.state;
        return (
            <div>
                <h1>Legendary</h1>
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
                        <div className='form-group'>
                            <div className='checkbox'>
                                <label htmlFor='includeSpecialSetupCards'>
                                    <input
                                        className='checkboxinput'
                                        type='checkbox'
                                        name='includeSpecialSetupCards'
                                        id='includeSpecialSetupCards'
                                        onChange={this.handleIncludeSpecialSetupCardsChange}
                                        value={includeSpecialSetupCards}
                                        disabled={generating}
                                        />
                                    Include dividers for special setup cards (Special Bystanders, Special Wounds, etc)
                                </label>
                            </div>
                    </div>
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
