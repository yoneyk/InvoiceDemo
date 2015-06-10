var assert = require('assert'),
    fs = require('fs'),
    request = require('supertest'),
    app = 'http://localhost:8080';

describe('InvoiceDemo 1', function(){
    var session_url;
    
    it('Get cookie', function(done){
        request(app)
        .get('/')
        .set('Accept', 'text/html')
        .expect('Content-Type', /html/)
        .expect(200)
        .expect('set-cookie', /Location=.*; path=/)
        .expect(function(res) {
            session_url = unescape(res.headers['set-cookie'][0].slice(9, - 8));
        })
        .end(function(err, res){
            if (err) return done(err);
            done();
        });
    }),
    it('Verify cookie', function(done){
        request(app)
        .get('/')
        .set('Accept', 'application/json')
        .set('X-Referer', session_url)
        .expect('Content-Type', /json/)
        .expect(200)
        .expect(function(res) {
            assert.strictEqual(res.headers['set-cookie'], undefined);
        })
        .end(function(err, res){
            if (err) return done(err);
            done();
        });
    }),
    it('Verify that keys match', function(done){
        request(app)
        .get('/')
        .set('Accept', 'application/json')
        .set('X-Referer', session_url)
        .expect('Content-Type', /json/)
        .expect(200)
        .expect(function(res) {
            var resKeys = Object.keys(JSON.parse(res.text)).sort();
            var InvoicePageKeys = Object.keys(JSON.parse(fs.readFileSync('InvoicePage.json', 'utf8'))).sort();

            assert.strictEqual(
                JSON.stringify(resKeys).replace(',"_ver#c$","_ver#s"', ''),
                JSON.stringify(InvoicePageKeys)
            );
        })
        .end(function(err, res){
            if (err) return done(err);
            done();
        });
    });
})