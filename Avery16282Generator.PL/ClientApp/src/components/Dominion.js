import React, { Component } from 'react';

const State = {
    Initializing: 0,
    Selecting: 1,
    Generating: 3,
    Generated: 4,
    Error: 5
}

export class Dominion extends Component {
    displayName = Dominion.name

    constructor(props) {
        super(props);
        this.state = {
            state: State.Initializing,
            error: null,
            downloadLink: null,
            allExpansions: [],
            selectedExpansions: [],
            useAllCards: true,
            selectedCards: [],
            labelsToSkip: 0
        };
        fetch('api/PdfGenerator/GetDominionExpansions')
            .then(response => response.json())
            .then(data => {
                this.setState({ allExpansions: data, state: State.Selecting });
            });
    }

    handleExpansionsChanged = (event) => {
        const { selectedExpansions: oldSelectedExpansions } = this.state;
        const newSelectedExpansions = Array.from(event.target.selectedOptions).map(option => option.value);
        const addedExpansions = newSelectedExpansions.filter(selectedExpansion => !oldSelectedExpansions.includes(selectedExpansion));
        const addedCards = addedExpansions.flatMap(addedExpansion => this.state.allExpansions.find(expansion => expansion.name === addedExpansion).cards.map(card => this.formatCardIdentifier(card)));
        const removedExpansions = oldSelectedExpansions.filter(selectedExpansion => !newSelectedExpansions.includes(selectedExpansion));
        const removedCards = removedExpansions.flatMap(removedExpansion => this.state.allExpansions.find(expansion => expansion.name === removedExpansion).cards.map(card => this.formatCardIdentifier(card)));
        const newSelectedCards = this.state.selectedCards.filter(selectedCard => !removedCards.includes(selectedCard)).concat(addedCards);
        
        this.setState({ selectedExpansions: newSelectedExpansions, selectedCards: newSelectedCards });
    }

    handleGenerateClicked = () => {
        const { selectedCards, allExpansions, selectedExpansions, labelsToSkip } = this.state;
        const availableCardIdentifiers = allExpansions
            .filter(expansion => selectedExpansions.includes(expansion.name))
            .flatMap(expansion => expansion.cards);
        this.setState({ state: State.Generating, error: null, downloadLink: null });
        const selectedCardIdentifiers = selectedCards.map(selectedCard => availableCardIdentifiers.find(availableCardIdentifier => this.formatCardIdentifier(availableCardIdentifier) === selectedCard));
        const body = JSON.stringify({
            selectedCardIdentifiers,
            labelsToSkip
        });
        fetch('api/PdfGenerator/GenerateDominion', {
            method: 'POST',
            body,
            headers: { 'Content-Type': 'application/json', },
        }).then(response => {
            if (response.ok) {
                response.text().then(data => this.setState({ state: State.Generated, downloadLink: data }));
            }
            else
                this.setState({ state: State.Error, error: response.statusText });
        }, error => {
            this.setState({ state: State.Error, error });
        });
    }
    
    handleSelectedCardsChanged = (event) => {
        const selectedCards = Array.from(event.target.selectedOptions).map(option => option.value);
        this.setState({ selectedCards });
    }
    
    formatCardIdentifier = (cardIdentifier) => {
        return `${cardIdentifier.cardSetName} - ${cardIdentifier.text}`;
    }

    render() {
        const {
            state,
            allExpansions,
            selectedExpansions,
            selectedCards,
            downloadLink,
            error,
            labelsToSkip
        } = this.state;
        const availableCardIdentifiers = allExpansions
            .filter(expansion => selectedExpansions.includes(expansion.name))
            .flatMap(expansion => expansion.cards);
        const buttonClass = (state === State.Generating || selectedCards.length === 0) ? 'btn btn-primary disabled' : 'btn btn-primary';
        return (
            <div>
                <h1>Dominion</h1>
                {state === State.Initializing && <p><em>Initializing...</em></p>}
                {state !== State.Initializing &&
                    <div>
                        <h3>Expansions</h3>
                        <select
                            disabled={state === State.Generating}
                            className='selectmultiple form-control'
                            multiple={true}
                            size={allExpansions.length}
                            onChange={this.handleExpansionsChanged}
                            value={selectedExpansions}>
                            {allExpansions.map((expansion => <option key={expansion.name}>{expansion.name}</option>))}
                        </select>
                        <h3>Cards</h3>
                        <select
                            disabled={state === State.Generating}
                            className='selectmultiple form-control'
                            multiple={true}
                            size={allExpansions.length}
                            onChange={this.handleSelectedCardsChanged}
                            value={selectedCards}>
                            {availableCardIdentifiers.map((availableCardIdentifier => <option key={this.formatCardIdentifier(availableCardIdentifier)}>{this.formatCardIdentifier(availableCardIdentifier)}</option>))}
                        </select>
                        <div>
                            <h3>Label Spots to Skip</h3>
                            <input type={'number'} onChange={(event) => this.setState({ labelsToSkip: +event.target.value })} value={labelsToSkip} />
                        </div>
                        <div>
                            <button type='button' className={buttonClass} onClick={this.handleGenerateClicked}>
                                {state === state.Generated && <div>Generating...</div>}
                                {state !== state.Generating && <div>Generate Labels</div>}
                            </button>
                        </div>
                        {downloadLink != null && <h3>Generated File: <a target="_blank" href={downloadLink}>Link</a> (Link valid for 1 day)</h3>}
                        {error != null && <div>Error generating file or uploading to S3: {error}</div>}
                    </div>
                }
            </div>
        );
    }
}
