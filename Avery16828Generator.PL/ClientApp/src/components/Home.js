import React, { Component } from 'react';
import { LinkContainer } from 'react-router-bootstrap';

export class Home extends Component {
  displayName = Home.name

  render() {
    return (
      <div>
        <h1>Avery 16282 Labels</h1>
            <p>This site contains generators for PDF files to print onto <a href="https://www.avery.com/products/tabs/16282">https://www.avery.com/products/tabs/16282</a>.</p>
            <p>They can be attached to plastic dividers or envelopes for various games to provide organization.</p>
            
           <div className='list-group'>
              <LinkContainer to={'/Brewcrafters'}>
                <a className='list-group-item'>
                    <h4 className='list-group-item-heading'>Brewcrafters</h4>
                    <p className='list-group-item-text'>Generates one label for each beer. These can be used to organize the beer tokens along with the gold label and card.</p>
                 </a>
                </LinkContainer>
                <LinkContainer to={'/Dominion'}>
                    <a className='list-group-item'>
                        <h4 className='list-group-item-heading'>Dominion</h4>
                        <p className='list-group-item-text'>Generates one label per suppy pile and setup pile. Supports choosing which expansions to include.</p>
                    </a>
                </LinkContainer>
                <LinkContainer to={'/AeonsEnd'}>
                    <a className='list-group-item'>
                        <h4 className='list-group-item-heading'>Aeon's End</h4>
                        <p className='list-group-item-text'>Generates labels for each gem, relic, spell and boss. Supports choosing which expansions to include.</p>
                    </a>
                </LinkContainer>
                <LinkContainer to={'/Legendary'}>
                    <a className='list-group-item'>
                        <h4 className='list-group-item-heading'>Legendary</h4>
                        <p className='list-group-item-text'>Generates labels for each Mastermind, Hero, Villan, Henchmen, and each type of setup and starting card. Supports choosing which expansions to include. Also supports optionally creating labels for special cards (i.e. special wounds, special bystanders, etc).</p>
                    </a>
                </LinkContainer>
          </div>
        </div>
    );
  }
}
